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

                    // Añadimos ORDER BY e_codigo ASC para el archivo descargado
                    string queryParticipantes = "SELECT p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_comentario, p_practica " +
                                                "FROM Equipo_participa WHERE p_torneo = ? ORDER BY e_codigo ASC, p_nombre ASC";

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
                    string queryParticipantes = "SELECT e_codigo, p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_comentario, p_practica " +
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
            sb.AppendLine("<tr><th>Nombre</th><th>Apellido</th><th>Movil</th><th>Asistencia</th><th>Transporte</th><th>Alergia</th><th>Comentario</th><th>Practica</th></tr>");

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_nombre"])}</td>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_apellido"])}</td>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_movi"])}</td>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_asistencia"])}</td>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_transporte"])}</td>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_alergia"])}</td>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_comentario"])}</td>");
                sb.AppendLine($"<td>{EscaparHtml(row["p_practica"])}</td>");
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
    }
}