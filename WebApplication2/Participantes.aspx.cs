using System;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Web.UI;

namespace WebApplication2
{
    public partial class Participantes : System.Web.UI.Page
    {
        private string connectionString
        {
            get
            {
                string rutaDB = Server.MapPath("~/Database/Torneo.accdb");
                return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={rutaDB};Persist Security Info=False;";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarParticipantes();
            }
        }

        protected void btnDescargar_Click(object sender, EventArgs e)
        {
            string torneoCodigo = Request.QueryString["torneo"];

            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se ha seleccionado ningún torneo válido.");
                return;
            }

            DataTable dt = new DataTable();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    bool tieneFechaMod = ExisteColumna(conn, "Equipo_participa", "Fecha_ult_modificacion");
                    string columnaFecha = tieneFechaMod ? ", p.Fecha_ult_modificacion" : string.Empty;

                    // Añadimos ORDER BY e_codigo ASC para el archivo descargado
                    string queryParticipantes = "SELECT e.e_nombre AS equipo_nombre, p.e_codigo, p.p_nombre, p.p_apellido, p.p_movi, p.p_asistencia, p.p_transporte, p.p_alergia, p.p_comentario, p.p_practica" +
                                                columnaFecha +
                                                " FROM Equipo_participa p LEFT JOIN Equipo e ON p.e_codigo = e.e_codigo WHERE p.p_torneo = ? ORDER BY p.e_codigo ASC, p.p_nombre ASC";

                    using (OleDbCommand cmdPart = new OleDbCommand(queryParticipantes, conn))
                    {
                        cmdPart.Parameters.AddWithValue("?", torneoCodigo);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdPart);
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al descargar los participantes: " + ex.Message);
                return;
            }

            if (dt.Rows.Count == 0)
            {
                MostrarMensajeError("No hay participantes para descargar.");
                return;
            }

            string nombreArchivo = $"participantes_{torneoCodigo}.xls";
            string contenidoExcel = ConvertirDataTableAExcel(dt);

            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");
            Response.Write("\uFEFF");
            Response.Write(contenidoExcel);
            Response.Flush();
            Response.End();
        }

        private void CargarParticipantes()
        {
            string torneoCodigo = Request.QueryString["torneo"];

            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se ha seleccionado ningún torneo válido.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // 1. Obtener el nombre del torneo
                    string queryTorneo = "SELECT t_nombre FROM Torneos WHERE t_codigo = ?";
                    using (OleDbCommand cmdTorneo = new OleDbCommand(queryTorneo, conn))
                    {
                        cmdTorneo.Parameters.AddWithValue("?", torneoCodigo);
                        object nombreResultado = cmdTorneo.ExecuteScalar();
                        if (nombreResultado != null)
                        {
                            lblNombreTorneo.Text = nombreResultado.ToString();
                        }
                        else
                        {
                            lblNombreTorneo.Text = "Torneo Desconocido";
                        }
                    }

                    // 2. Extraer a los participantes
                    DataTable dt = new DataTable();
                    // Incluimos e_codigo en el SELECT y ordenamos por e_codigo y nombre
                    string queryParticipantes = "SELECT p_contador, e_codigo, p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_comentario, p_practica " +
                                                "FROM Equipo_participa WHERE p_torneo = ? ORDER BY e_codigo ASC, p_nombre ASC";

                    using (OleDbCommand cmdPart = new OleDbCommand(queryParticipantes, conn))
                    {
                        cmdPart.Parameters.AddWithValue("?", torneoCodigo);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdPart);
                        adapter.Fill(dt);
                    }

                    // 3. Pintar los datos
                    if (dt.Rows.Count > 0)
                    {
                        rptParticipantes.DataSource = dt;
                        rptParticipantes.DataBind();
                        pnlNoData.Visible = false;
                    }
                    else
                    {
                        rptParticipantes.DataSource = null;
                        rptParticipantes.DataBind();
                        pnlNoData.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al cargar los participantes: " + ex.Message);
            }
        }

        protected void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            try
            {
                int cambiosGuardados = 0;
                string torneoCodigo = Request.QueryString["torneo"];

                if (string.IsNullOrEmpty(torneoCodigo))
                {
                    MostrarMensajeError("No se ha seleccionado ningún torneo válido.");
                    return;
                }

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE Equipo_participa
                             SET p_nombre = ?,
                                 p_apellido = ?,
                                 p_torneo = ?,
                                 e_codigo = ?,
                                 p_movi = ?,
                                 p_asistencia = ?,
                                 p_alergia = ?,
                                 p_transporte = ?,
                                 p_comentario = ?
                             WHERE p_contador = ?";

                    foreach (System.Web.UI.WebControls.RepeaterItem item in rptParticipantes.Items)
                    {
                        if (item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                            item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                        {
                            continue;
                        }

                        var hdnContador = (System.Web.UI.WebControls.HiddenField)item.FindControl("hdnContador");
                        var txtEditNombre = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditNombre");
                        var txtEditApellido = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditApellido");
                        var txtEditEquipo = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditEquipo");
                        var txtEditMovil = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditMovil");
                        var txtEditAlergias = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditAlergias");
                        var rblEditAsistencia = (System.Web.UI.WebControls.RadioButtonList)item.FindControl("rblEditAsistencia");
                        var rblEditTransporte = (System.Web.UI.WebControls.RadioButtonList)item.FindControl("rblEditTransporte");
                        var txtEditComentario = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditComentario");

                        if (hdnContador == null || txtEditNombre == null || txtEditApellido == null || txtEditEquipo == null ||
                            txtEditMovil == null || txtEditAlergias == null || rblEditAsistencia == null ||
                            rblEditTransporte == null || txtEditComentario == null)
                        {
                            continue;
                        }

                        int id;
                        if (!int.TryParse(hdnContador.Value, out id))
                        {
                            continue;
                        }

                        string nombre = (Request.Form[txtEditNombre.UniqueID] ?? txtEditNombre.Text ?? string.Empty).Trim();
                        string apellido = (Request.Form[txtEditApellido.UniqueID] ?? txtEditApellido.Text ?? string.Empty).Trim();
                        string equipo = (Request.Form[txtEditEquipo.UniqueID] ?? txtEditEquipo.Text ?? string.Empty).Trim().ToUpper();
                        string movil = (Request.Form[txtEditMovil.UniqueID] ?? txtEditMovil.Text ?? string.Empty).Trim();
                        string asistencia = rblEditAsistencia.SelectedValue;
                        string alergia = (Request.Form[txtEditAlergias.UniqueID] ?? txtEditAlergias.Text ?? string.Empty).Trim();
                        string transporte = rblEditTransporte.SelectedValue;
                        string comentario = (Request.Form[txtEditComentario.UniqueID] ?? txtEditComentario.Text ?? string.Empty).Trim();

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@p_nombre", string.IsNullOrEmpty(nombre) ? (object)DBNull.Value : nombre);
                            cmd.Parameters.AddWithValue("@p_apellido", string.IsNullOrEmpty(apellido) ? (object)DBNull.Value : apellido);
                            cmd.Parameters.AddWithValue("@p_torneo", torneoCodigo);
                            cmd.Parameters.AddWithValue("@e_codigo", string.IsNullOrEmpty(equipo) ? (object)DBNull.Value : equipo);
                            cmd.Parameters.AddWithValue("@p_movi", string.IsNullOrEmpty(movil) ? (object)DBNull.Value : movil);
                            cmd.Parameters.AddWithValue("@p_asistencia", string.IsNullOrEmpty(asistencia) ? (object)DBNull.Value : asistencia);
                            cmd.Parameters.AddWithValue("@p_alergia", string.IsNullOrEmpty(alergia) ? (object)DBNull.Value : alergia);
                            cmd.Parameters.AddWithValue("@p_transporte", string.IsNullOrEmpty(transporte) ? (object)DBNull.Value : transporte);
                            cmd.Parameters.AddWithValue("@p_comentario", string.IsNullOrEmpty(comentario) ? (object)DBNull.Value : comentario);
                            cmd.Parameters.AddWithValue("@p_contador", id);

                            cambiosGuardados += cmd.ExecuteNonQuery();
                        }
                    }
                }

                if (cambiosGuardados > 0)
                {
                    MostrarMensajeExito("Cambios guardados correctamente: " + cambiosGuardados + " registro(s).");
                }
                else
                {
                    MostrarMensajeError("No se actualizó ningún registro. Revisa que los IDs (p_contador) existan.");
                }

                CargarParticipantes();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar cambios: " + ex.Message);
            }
        }

        protected void rptParticipantes_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "EliminarParticipante", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string torneoCodigo = Request.QueryString["torneo"];
            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se ha seleccionado ningún torneo válido.");
                return;
            }

            int idParticipante;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out idParticipante))
            {
                MostrarMensajeError("No se pudo identificar el participante a eliminar.");
                return;
            }

            try
            {
                int filasAfectadas = 0;

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM Equipo_participa WHERE p_contador = ? AND p_torneo = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", idParticipante);
                        cmd.Parameters.AddWithValue("?", torneoCodigo);
                        filasAfectadas = cmd.ExecuteNonQuery();
                    }
                }

                if (filasAfectadas > 0)
                {
                    MostrarMensajeExito("Participante eliminado correctamente.");
                }
                else
                {
                    MostrarMensajeError("No se eliminó ningún registro.");
                }

                CargarParticipantes();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al eliminar participante: " + ex.Message);
            }
        }

        private bool ExisteColumna(OleDbConnection conn, string tabla, string columna)
        {
            string query = "SELECT TOP 1 * FROM " + tabla;
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            using (OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                DataTable schema = reader.GetSchemaTable();
                if (schema == null)
                {
                    return false;
                }

                foreach (DataRow fila in schema.Rows)
                {
                    string nombreColumna = Convert.ToString(fila["ColumnName"]);
                    if (string.Equals(nombreColumna, columna, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void btnGuardarParticipante_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string torneoCodigo = Request.QueryString["torneo"];

            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se puede guardar un participante sin un código de torneo válido.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // --- NUEVA VALIDACIÓN: COMPROBAR SI EL CÓDIGO DE EQUIPO EXISTE ---
                    string codEquipoIngresado = txtAddCodigoEquipo.Text.Trim().ToUpper();
                    bool equipoExiste = false;

                    string queryValidarEquipo = "SELECT COUNT(*) FROM Equipo_participa WHERE e_codigo = ?";
                    using (OleDbCommand cmdValidar = new OleDbCommand(queryValidarEquipo, conn))
                    {
                        cmdValidar.Parameters.AddWithValue("?", codEquipoIngresado);
                        int count = Convert.ToInt32(cmdValidar.ExecuteScalar());
                        if (count > 0)
                        {
                            equipoExiste = true;
                        }
                    }

                    // Si no existe no permitimos guardar el participante y mostramos un mensaje de error
                    if (!equipoExiste)
                    {
                        MostrarMensajeError($"El código de equipo '{codEquipoIngresado}' no existe en la base de datos. Por favor, introduzca uno válido.");
                        return;
                    }

                    // Consulta de inserción incluyendo e_codigo
                    string query = @"INSERT INTO Equipo_participa 
                                     (p_torneo, e_codigo, p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_practica, p_comentario) 
                                     VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@p_torneo", torneoCodigo);
                        cmd.Parameters.AddWithValue("@e_codigo", codEquipoIngresado);
                        cmd.Parameters.AddWithValue("@p_nombre", txtAddNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@p_apellido", txtAddApellido.Text.Trim());

                        cmd.Parameters.AddWithValue("@p_movi", string.IsNullOrEmpty(txtAddMovil.Text.Trim()) ? (object)DBNull.Value : txtAddMovil.Text.Trim());
                        cmd.Parameters.AddWithValue("@p_asistencia", string.IsNullOrEmpty(rblAddAsistencia.SelectedValue) ? (object)DBNull.Value : rblAddAsistencia.SelectedValue);
                        cmd.Parameters.AddWithValue("@p_transporte", string.IsNullOrEmpty(rblAddTransporte.SelectedValue) ? (object)DBNull.Value : rblAddTransporte.SelectedValue);
                        cmd.Parameters.AddWithValue("@p_alergia", string.IsNullOrEmpty(txtAddAlergia.Text.Trim()) ? (object)DBNull.Value : txtAddAlergia.Text.Trim());
                        cmd.Parameters.AddWithValue("@p_practica", string.IsNullOrEmpty(rblAddPractica.SelectedValue) ? (object)DBNull.Value : rblAddPractica.SelectedValue);
                        cmd.Parameters.AddWithValue("@p_comentario", string.IsNullOrEmpty(txtAddComentario.Text.Trim()) ? (object)DBNull.Value : txtAddComentario.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                // Mostrar éxito
                MostrarMensajeExito("¡El participante ha sido añadido con éxito!");

                // Limpiar el formulario
                LimpiarFormularioAñadir();

                // Recargar el grid para que aparezca el nuevo registro en la pantalla
                CargarParticipantes();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar el participante: " + ex.Message);
            }
        }

        private void LimpiarFormularioAñadir()
        {
            txtAddNombre.Text = string.Empty;
            txtAddApellido.Text = string.Empty;
            txtAddCodigoEquipo.Text = string.Empty;
            txtAddMovil.Text = string.Empty;
            txtAddAlergia.Text = string.Empty;
            txtAddComentario.Text = string.Empty;

            rblAddAsistencia.ClearSelection();
            rblAddTransporte.ClearSelection();
            rblAddPractica.ClearSelection();
        }

        private void MostrarMensajeExito(string mensaje)
        {
            lblMensajeExito.Text = mensaje;
            pnlMensajeExito.Visible = true;
            pnlMensajeError.Visible = false;
        }

        private void MostrarMensajeError(string mensaje)
        {
            lblMensajeError.Text = mensaje;
            pnlMensajeError.Visible = true;
            pnlMensajeExito.Visible = false;
        }

        private static string ConvertirDataTableAExcel(DataTable dt)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table border='1'>");
            sb.AppendLine("<tr><th>Equipo</th><th>Codigo Equipo</th><th>Nombre</th><th>Apellido</th><th>Movil</th><th>Asistencia</th><th>Transporte</th><th>Alergia</th><th>Comentario</th><th>Practica</th><th>Fecha Ult Modificacion</th></tr>");

            const string colEquipoNombre = "equipo_nombre";
            const string colEquipoCodigo = "e_codigo";
            const string colNombre = "p_nombre";
            const string colApellido = "p_apellido";
            const string colMovil = "p_movi";
            const string colAsistencia = "p_asistencia";
            const string colTransporte = "p_transporte";
            const string colAlergia = "p_alergia";
            const string colComentario = "p_comentario";
            const string colPractica = "p_practica";
            const string colFechaMod = "Fecha_ult_modificacion";

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colEquipoNombre)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colEquipoCodigo)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colNombre)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colApellido)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colMovil)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colAsistencia)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colTransporte)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colAlergia)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colComentario)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colPractica)}</td>");
                sb.AppendLine($"<td>{ObtenerCelda(row, colFechaMod)}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        private static string EscaparHtml(object valor)
        {
            if (valor == null || valor == DBNull.Value)
            {
                return string.Empty;
            }

            return System.Net.WebUtility.HtmlEncode(valor.ToString());
        }

        private static string ObtenerCelda(DataRow row, string columna)
        {
            if (!row.Table.Columns.Contains(columna))
            {
                return string.Empty;
            }

            return EscaparHtml(row[columna]);
        }
    }
}