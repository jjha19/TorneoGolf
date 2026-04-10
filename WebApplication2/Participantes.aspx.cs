using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

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
                    string queryParticipantes = "SELECT p.p_contador, p.e_codigo, e.e_nombre AS equipo_nombre, e.e_cerrado, p.p_nombre, p.p_apellido, p.p_movi, p.p_asistencia, p.p_transporte, p.p_alergia, p.p_comentario, p.p_practica, p.fecha_ws " +
                                                "FROM Equipo_participa p LEFT JOIN Equipo e ON p.e_codigo = e.e_codigo WHERE p.p_torneo = ? ORDER BY p.e_codigo ASC, p.p_nombre ASC";

                    using (OleDbCommand cmdPart = new OleDbCommand(queryParticipantes, conn))
                    {
                        cmdPart.Parameters.AddWithValue("?", torneoCodigo);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdPart);
                        adapter.Fill(dt);
                    }

                    // 3. Pintar los datos
                    if (dt.Rows.Count > 0)
                    {
                        lblInvitadosTotal.Text = dt.Rows.Count.ToString();
                        int totalSi = 0;
                        int totalNo = 0;
                        int totalPendiente = 0;
                        int totalPracticaSi = 0;
                        int totalPracticaNo = 0;
                        int totalPracticaPendiente = 0;
                        int totalTransporteSi = 0;
                        int totalTransporteNo = 0;
                        int totalTransportePendiente = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            string asistencia = Convert.ToString(row["p_asistencia"])?.Trim();
                            if (string.Equals(asistencia, "Si", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(asistencia, "Sí", StringComparison.OrdinalIgnoreCase))
                            {
                                totalSi++;
                            }
                            else if (string.Equals(asistencia, "No", StringComparison.OrdinalIgnoreCase))
                            {
                                totalNo++;
                            }
                            else
                            {
                                totalPendiente++;
                            }

                            if (!string.Equals(asistencia, "No", StringComparison.OrdinalIgnoreCase))
                            {
                                string practica = Convert.ToString(row["p_practica"])?.Trim();
                                if (string.Equals(practica, "Si", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(practica, "Sí", StringComparison.OrdinalIgnoreCase))
                                {
                                    totalPracticaSi++;
                                }
                                else if (string.Equals(practica, "No", StringComparison.OrdinalIgnoreCase))
                                {
                                    totalPracticaNo++;
                                }
                                else
                                {
                                    totalPracticaPendiente++;
                                }

                                string transporte = Convert.ToString(row["p_transporte"])?.Trim();
                                if (string.Equals(transporte, "Si", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(transporte, "Sí", StringComparison.OrdinalIgnoreCase))
                                {
                                    totalTransporteSi++;
                                }
                                else if (string.Equals(transporte, "No", StringComparison.OrdinalIgnoreCase))
                                {
                                    totalTransporteNo++;
                                }
                                else
                                {
                                    totalTransportePendiente++;
                                }
                            }
                        }

                        lblAsistenSi.Text = totalSi.ToString();
                        lblAsistenNo.Text = totalNo.ToString();
                        lblAsistenPendiente.Text = totalPendiente.ToString();
                        lblPracticaSi.Text = totalPracticaSi.ToString();
                        lblPracticaNo.Text = totalPracticaNo.ToString();
                        lblPracticaPendiente.Text = totalPracticaPendiente.ToString();
                        lblTransporteSi.Text = totalTransporteSi.ToString();
                        lblTransporteNo.Text = totalTransporteNo.ToString();
                        lblTransportePendiente.Text = totalTransportePendiente.ToString();

                        ViewState["EquipoActual"] = null;
                        rptParticipantes.DataSource = dt;
                        rptParticipantes.DataBind();
                        pnlNoData.Visible = false;
                    }
                    else
                    {
                        lblInvitadosTotal.Text = "0";
                        lblAsistenSi.Text = "0";
                        lblAsistenNo.Text = "0";
                        lblAsistenPendiente.Text = "0";
                        lblPracticaSi.Text = "0";
                        lblPracticaNo.Text = "0";
                        lblPracticaPendiente.Text = "0";
                        lblTransporteSi.Text = "0";
                        lblTransporteNo.Text = "0";
                        lblTransportePendiente.Text = "0";
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

        protected void rptParticipantes_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                return;
            }

            var itemContainer = e.Item.FindControl("itemContainer") as HtmlGenericControl;
            if (itemContainer == null)
            {
                return;
            }

            string equipoCodigo = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "e_codigo"))?.Trim();
            string equipoNombre = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "equipo_nombre"))?.Trim();
            string asistenciaValor = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "p_asistencia"))?.Trim();
            var phEquipoHeader = e.Item.FindControl("phEquipoHeader") as System.Web.UI.WebControls.PlaceHolder;
            string equipoAnterior = ViewState["EquipoActual"] as string;

            if (phEquipoHeader != null && !string.Equals(equipoAnterior, equipoCodigo, StringComparison.OrdinalIgnoreCase))
            {
                string tituloEquipo = string.IsNullOrWhiteSpace(equipoNombre)
                    ? $"Equipo: {equipoCodigo}"
                    : $"{equipoNombre} - {equipoCodigo}";
                string cerradoValor = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "e_cerrado"))?.Trim();
                bool invitacionCerrada = string.Equals(cerradoValor, "1", StringComparison.OrdinalIgnoreCase);
                string checkedAttr = invitacionCerrada ? "checked='checked'" : string.Empty;
                string inputName = "chkCerrado_" + equipoCodigo;
                string headerHtml = $"<li class='equipo-header'><div class='equipo-header-row'>" +
                                    $"<span class='equipo-titulo'>{System.Net.WebUtility.HtmlEncode(tituloEquipo)}</span>" +
                                    $"<div class='checkbox-con'><label>Invitacion Cerrada</label><input type='checkbox' name='{System.Net.WebUtility.HtmlEncode(inputName)}' {checkedAttr} /></div>" +
                                    "</div></li>";
                phEquipoHeader.Controls.Add(new LiteralControl(headerHtml));
                ViewState["EquipoActual"] = equipoCodigo;
            }

            if (string.IsNullOrEmpty(equipoCodigo))
            {
                return;
            }

            string color = ObtenerColorEquipo(equipoCodigo);
            string estiloActual = itemContainer.Attributes["style"] ?? string.Empty;
            itemContainer.Attributes["style"] = $"{estiloActual}border-right: 8px solid {color}; --equipo-color: {color};";

            if (string.Equals(asistenciaValor, "Si", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(asistenciaValor, "Sí", StringComparison.OrdinalIgnoreCase))
            {
                itemContainer.Attributes["class"] = (itemContainer.Attributes["class"] + " asistencia-si").Trim();
            }
            else if (string.Equals(asistenciaValor, "No", StringComparison.OrdinalIgnoreCase))
            {
                itemContainer.Attributes["class"] = (itemContainer.Attributes["class"] + " asistencia-no").Trim();
            }

            var txtEditFechaWs = e.Item.FindControl("txtEditFechaWs") as System.Web.UI.WebControls.TextBox;
            if (txtEditFechaWs != null)
            {
                string fechaRaw = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "fecha_ws"));
                if (string.IsNullOrWhiteSpace(fechaRaw))
                {
                    txtEditFechaWs.Text = "Todavía no se ha enviado";
                }
                else if (DateTime.TryParseExact(fechaRaw, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaValor))
                {
                    txtEditFechaWs.Text = fechaValor.ToString("dd/MM/yyyy HH:mm:ss");
                }
                else
                {
                    txtEditFechaWs.Text = fechaRaw;
                }
            }
        }

        private static string ObtenerColorEquipo(string equipoCodigo)
        {
            string[] colores =
            {
                "#FFA500",
                "#22C55E",
                "#8B5CF6",
                "#38BDF8",
                "#EC4899",
                "#2563EB",
                "#EF4444"
            };

            int hash = equipoCodigo.GetHashCode();
            int index = Math.Abs(hash) % colores.Length;
            return colores[index];
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

                    var equiposProcesados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    string query = @"UPDATE Equipo_participa
                             SET p_nombre = ?,
                                 p_apellido = ?,
                                 p_torneo = ?,
                                 e_codigo = ?,
                                 p_movi = ?,
                                 p_asistencia = ?,
                                 p_practica = ?,
                                 p_alergia = ?,
                                 p_transporte = ?,
                                 p_comentario = ?,
                                 fecha_ws = ?
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
                        var rblEditPractica = (System.Web.UI.WebControls.RadioButtonList)item.FindControl("rblEditPractica");
                        var txtEditComentario = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditComentario");
                        var txtEditFechaWs = (System.Web.UI.WebControls.TextBox)item.FindControl("txtEditFechaWs");

                        if (hdnContador == null || txtEditNombre == null || txtEditApellido == null || txtEditEquipo == null ||
                            txtEditMovil == null || txtEditAlergias == null || rblEditAsistencia == null ||
                            rblEditTransporte == null || rblEditPractica == null || txtEditComentario == null || txtEditFechaWs == null)
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
                        string practica = rblEditPractica.SelectedValue;
                        string alergia = (Request.Form[txtEditAlergias.UniqueID] ?? txtEditAlergias.Text ?? string.Empty).Trim();
                        string transporte = rblEditTransporte.SelectedValue;
                        string comentario = (Request.Form[txtEditComentario.UniqueID] ?? txtEditComentario.Text ?? string.Empty).Trim();
                        string fechaWsTexto = (Request.Form[txtEditFechaWs.UniqueID] ?? txtEditFechaWs.Text ?? string.Empty).Trim();
                        object fechaWsValue = DBNull.Value;

                        if (!string.IsNullOrEmpty(fechaWsTexto) && !string.Equals(fechaWsTexto, "Todavía no se ha enviado", StringComparison.OrdinalIgnoreCase))
                        {
                            if (DateTime.TryParseExact(fechaWsTexto, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                            {
                                fechaWsValue = fechaWsTexto;
                            }
                        }

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@p_nombre", string.IsNullOrEmpty(nombre) ? (object)DBNull.Value : nombre);
                            cmd.Parameters.AddWithValue("@p_apellido", string.IsNullOrEmpty(apellido) ? (object)DBNull.Value : apellido);
                            cmd.Parameters.AddWithValue("@p_torneo", torneoCodigo);
                            cmd.Parameters.AddWithValue("@e_codigo", string.IsNullOrEmpty(equipo) ? (object)DBNull.Value : equipo);
                            cmd.Parameters.AddWithValue("@p_movi", string.IsNullOrEmpty(movil) ? (object)DBNull.Value : movil);
                            cmd.Parameters.AddWithValue("@p_asistencia", string.IsNullOrEmpty(asistencia) ? (object)DBNull.Value : asistencia);
                            cmd.Parameters.AddWithValue("@p_practica", string.IsNullOrEmpty(practica) ? (object)DBNull.Value : practica);
                            cmd.Parameters.AddWithValue("@p_alergia", string.IsNullOrEmpty(alergia) ? (object)DBNull.Value : alergia);
                            cmd.Parameters.AddWithValue("@p_transporte", string.IsNullOrEmpty(transporte) ? (object)DBNull.Value : transporte);
                            cmd.Parameters.AddWithValue("@p_comentario", string.IsNullOrEmpty(comentario) ? (object)DBNull.Value : comentario);
                            cmd.Parameters.AddWithValue("@fecha_ws", fechaWsValue);
                            cmd.Parameters.AddWithValue("@p_contador", id);

                            cambiosGuardados += cmd.ExecuteNonQuery();
                        }

                        if (!string.IsNullOrEmpty(equipo) && equiposProcesados.Add(equipo))
                        {
                            string inputName = "chkCerrado_" + equipo;
                            bool invitacionCerrada = !string.IsNullOrEmpty(Request.Form[inputName]);

                            string queryActualizarCerrado = "UPDATE Equipo SET e_cerrado = ? WHERE e_codigo = ?";
                            using (OleDbCommand cmdActualizar = new OleDbCommand(queryActualizarCerrado, conn))
                            {
                                cmdActualizar.Parameters.AddWithValue("?", invitacionCerrada ? 1 : 0);
                                cmdActualizar.Parameters.AddWithValue("?", equipo);
                                cmdActualizar.ExecuteNonQuery();
                            }
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
            string torneoCodigo = Request.QueryString["torneo"];
            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se ha seleccionado ningún torneo válido.");
                return;
            }

            int idParticipante;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out idParticipante))
            {
                MostrarMensajeError("No se pudo identificar el participante.");
                return;
            }

            if (string.Equals(e.CommandName, "EnviarWhatsapp", StringComparison.OrdinalIgnoreCase))
            {
                var txtEditMovil = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtEditMovil");

                string movilRaw = txtEditMovil != null ? txtEditMovil.Text : string.Empty;
                string telefono = LimpiarTelefonoWhatsapp(movilRaw);

                if (string.IsNullOrEmpty(telefono))
                {
                    MostrarMensajeError("Este participante no tiene móvil válido para WhatsApp.");
                    return;
                }

                string mensaje;
                string linkPrincipal;

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    mensaje = ObtenerTextoWhatsappParticipante(conn, idParticipante, torneoCodigo);
                    linkPrincipal = ObtenerLinkPrincipalParticipante(conn, idParticipante);

                    string queryActualizarFecha = "UPDATE Equipo_participa SET fecha_ws = ? WHERE p_contador = ? AND p_torneo = ?";
                    using (OleDbCommand cmdActualizar = new OleDbCommand(queryActualizarFecha, conn))
                    {
                        string fechaTexto = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        cmdActualizar.Parameters.Add("?", OleDbType.VarWChar).Value = fechaTexto;
                        cmdActualizar.Parameters.Add("?", OleDbType.Integer).Value = idParticipante;
                        cmdActualizar.Parameters.Add("?", OleDbType.VarWChar).Value = torneoCodigo;
                        cmdActualizar.ExecuteNonQuery();
                    }
                }

                if (string.IsNullOrWhiteSpace(mensaje))
                {
                    MostrarMensajeError("El torneo no tiene texto de WhatsApp configurado (campo texto_ws).");
                    return;
                }

                string mensajeFinal = mensaje;

                if (!string.IsNullOrWhiteSpace(linkPrincipal))
                {
                    mensajeFinal += "\n\nConfirma la invitación desde aquí:\n" + linkPrincipal;
                }

                string url = "https://wa.me/" + telefono + "?text=" + Uri.EscapeDataString(mensajeFinal);
                Response.Redirect(url, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (string.Equals(e.CommandName, "EliminarParticipante", StringComparison.OrdinalIgnoreCase))
            {
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
        }

        private static string LimpiarTelefonoWhatsapp(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
            {
                return string.Empty;
            }

            telefono = telefono.Trim();
            var sb = new StringBuilder();

            for (int i = 0; i < telefono.Length; i++)
            {
                char c = telefono[i];
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            string limpio = sb.ToString();

            if (limpio.StartsWith("00"))
            {
                limpio = limpio.Substring(2);
            }

            // Si llega móvil nacional de 9 dígitos, asumimos España (34)
            if (limpio.Length == 9)
            {
                limpio = "34" + limpio;
            }

            return limpio;
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

        private string ObtenerTextoWhatsappParticipante(OleDbConnection conn, int idParticipante, string torneoCodigo)
        {
            string mensaje = string.Empty;

            // 1) Obtener texto_ws por el torneo real del participante
            string query = @"SELECT TOP 1 t.texto_ws
                             FROM Equipo_participa p
                             INNER JOIN Torneos t ON p.p_torneo = t.t_codigo
                             WHERE p.p_contador = ?";

            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("?", idParticipante);
                object textoObj = cmd.ExecuteScalar();
                if (textoObj != null && textoObj != DBNull.Value)
                {
                    mensaje = textoObj.ToString();
                }
            }

            // 2) Fallback por código de torneo de la URL
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                query = "SELECT TOP 1 texto_ws FROM Torneos WHERE t_codigo = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", torneoCodigo);
                    object textoObj = cmd.ExecuteScalar();
                    if (textoObj != null && textoObj != DBNull.Value)
                    {
                        mensaje = textoObj.ToString();
                    }
                }
            }

            return mensaje;
        }

        private string ObtenerLinkPrincipalParticipante(OleDbConnection conn, int idParticipante)
        {
            string query = @"SELECT TOP 1 e.e_contador
                     FROM Equipo_participa p
                     INNER JOIN Equipo e ON p.e_codigo = e.e_codigo
                     WHERE p.p_contador = ?";

            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("?", idParticipante);

                object resultado = cmd.ExecuteScalar();
                if (resultado == null || resultado == DBNull.Value)
                {
                    return string.Empty;
                }

                int index;
                if (!int.TryParse(resultado.ToString(), out index))
                {
                    return string.Empty;
                }

                // Link para pruebas internas
                // return "https://localhost:44340/Principal.aspx?index=" + index;

                // Link para pruebas externas
                // (descomentar si se quiere usar en producción, pero asegurarse de que la IP
                // y puerto son correctos y accesibles desde internet)

                return "http://213.37.131.233:83/invitacion/Principal.aspx?index=" + index;
            }
        }
    }
}