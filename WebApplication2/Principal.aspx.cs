using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class Principal : System.Web.UI.Page
    {
        // Cadena de conexión - Base de datos Access en carpeta Database
        private string connectionString
        {
            get
            {
                // Obtener la ruta física de la carpeta Database
                string rutaDB = Server.MapPath("~/Database/Torneo.accdb");
                return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={rutaDB};Persist Security Info=False;";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["index"]))
            {
                Response.StatusCode = 404;
                Response.End();
                return;
            }

            if (!IsPostBack)
            {
                InicializarPagina();
                CargarIntegrantes();

                if (InvitacionCerrada)
                {
                    BloquearCamposInvitacion();
                    MostrarMensajeError("La invitación está cerrada. Ya no se pueden modificar los datos");
                }
            }
            else
            {
                if (InvitacionCerrada)
                {
                    BloquearCamposInvitacion();
                    MostrarMensajeError("La invitación está cerrada. Ya no se pueden modificar los datos");
                }
            }
        }

        private void InicializarPagina()
        {
            string nombreTorneo = "Torneo no encontrado";
            string comentarioTorneo = "Sin comentario disponible";
            string nombreEquipo = "Equipo no encontrado";
            string codigoEquipo = "AB124"; // Valor por defecto
            string torneoVinculado = "";
            bool invitacionCerrada = false;

            // Obtener el contador del equipo desde la URL (e_contador), por defecto 1
            int equipoContador = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["index"]))
            {
                int.TryParse(Request.QueryString["index"], out equipoContador);
                if (equipoContador < 1) equipoContador = 1;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Ahora también leemos e_cerrado
                    string queryEquipo = "SELECT e_nombre, e_codigo, e_torneo, e_cerrado FROM Equipo WHERE e_contador = ?";
                    using (OleDbCommand cmdEquipo = new OleDbCommand(queryEquipo, conn))
                    {
                        cmdEquipo.Parameters.AddWithValue("?", equipoContador);
                        using (OleDbDataReader readerEquipo = cmdEquipo.ExecuteReader())
                        {
                            if (readerEquipo.Read())
                            {
                                nombreEquipo = readerEquipo["e_nombre"].ToString().Trim();
                                codigoEquipo = readerEquipo["e_codigo"] == DBNull.Value ? "" : readerEquipo["e_codigo"].ToString().Trim();
                                torneoVinculado = readerEquipo["e_torneo"] == DBNull.Value ? "" : readerEquipo["e_torneo"].ToString().Trim();
                                invitacionCerrada = EsInvitacionCerrada(readerEquipo["e_cerrado"]);

                                Session["NombreEquipo"] = nombreEquipo;
                                Session["CodigoEquipo"] = codigoEquipo;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("No existe el equipo con e_contador = " + equipoContador);
                            }
                        }
                    }

                    // Si encontramos el torneo vinculado, buscamos su información
                    if (!string.IsNullOrEmpty(torneoVinculado))
                    {
                        string queryTorneo = "SELECT t_nombre, t_comen FROM Torneos WHERE t_codigo = ?";
                        using (OleDbCommand cmdTorneo = new OleDbCommand(queryTorneo, conn))
                        {
                            cmdTorneo.Parameters.AddWithValue("?", torneoVinculado);
                            using (OleDbDataReader readerTorneo = cmdTorneo.ExecuteReader())
                            {
                                if (readerTorneo.Read())
                                {
                                    nombreTorneo = readerTorneo["t_nombre"].ToString().Trim();
                                    comentarioTorneo = readerTorneo["t_comen"] == DBNull.Value ? "" : readerTorneo["t_comen"].ToString().Trim();
                                    Session["NombreTorneo"] = nombreTorneo;
                                }
                                else
                                {
                                    nombreTorneo = "Código no encontrado: " + torneoVinculado;
                                    System.Diagnostics.Debug.WriteLine("No se encontró información extra para el torneo con código: " + torneoVinculado);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al inicializar la página: " + ex.Message);
            }

            InvitacionCerrada = invitacionCerrada;

            // Asignar valores a los labels del título
            lblTituloTorneo.Text = nombreTorneo;
            lblTituloEquipo.Text = nombreEquipo;
            lblComentarioTorneo.Text = comentarioTorneo;
        }

        private void CargarIntegrantes()
        {
            try
            {
                DataTable dt = new DataTable();

                // Obtener el código de equipo de la sesión (seteado en InicializarPagina)
                string codigoEquipo = Session["CodigoEquipo"]?.ToString() ?? "AB124";

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string query = "SELECT p_contador, p_nombre, p_apellido, p_asistencia, p_transporte, p_alergia, p_practica, p_comentario FROM Equipo_participa WHERE e_codigo = ?";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@e_codigo", codigoEquipo);

                    conn.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    adapter.Fill(dt);

                    // Usar el primer registro para rellenar la pregunta de práctica
                    if (dt.Rows.Count > 0)
                    {
                        object practicaValor = dt.Rows[0]["p_practica"];
                        string practica = practicaValor == DBNull.Value ? null : practicaValor.ToString().Trim();

                        if (!string.IsNullOrEmpty(practica) && rblPractica != null)
                        {
                            // Normalizamos "Sí" y "Si" a "Si" para SelectedValue
                            if (practica.Equals("Sí", StringComparison.OrdinalIgnoreCase))
                                practica = "Si";
                            rblPractica.SelectedValue = practica;
                        }

                        object comentarioValor = dt.Rows[0]["p_comentario"];
                        string comentario = comentarioValor == DBNull.Value ? string.Empty : comentarioValor.ToString().Trim();
                        txtComentario.Text = comentario;
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    rptIntegrantes.DataSource = dt;
                    rptIntegrantes.DataBind();
                    pnlNoData.Visible = false;
                }
                else
                {
                    rptIntegrantes.DataSource = null;
                    rptIntegrantes.DataBind();
                    pnlNoData.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al cargar integrantes: " + ex.Message);
            }
        }

        protected void rptIntegrantes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            AplicarVisibilidadAsistencia(e.Item);
        }

        protected void rblAsistencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvitacionCerrada)
            {
                MostrarMensajeError("La invitación está cerrada.");
                return;
            }

            RadioButtonList rblAsistencia = sender as RadioButtonList;
            RepeaterItem item = rblAsistencia != null ? rblAsistencia.NamingContainer as RepeaterItem : null;

            if (item == null)
            {
                return;
            }

            AplicarVisibilidadAsistencia(item);
        }

        private void AplicarVisibilidadAsistencia(RepeaterItem item)
        {
            var rblAsistencia = (RadioButtonList)item.FindControl("rblAsistencia");
            var rblTransporte = (RadioButtonList)item.FindControl("rblTransporte");
            var txtAlergia = (TextBox)item.FindControl("txtAlergia");
            var pnlTransporte = (Panel)item.FindControl("pnlTransporte");
            var pnlAlergia = (Panel)item.FindControl("pnlAlergia");
            var rfvTransporte = (RequiredFieldValidator)item.FindControl("rfvTransporte");
            var itemContainer = (HtmlGenericControl)item.FindControl("itemContainer");

            bool ocultarExtras = string.Equals(rblAsistencia?.SelectedValue, "No", StringComparison.OrdinalIgnoreCase);

            if (pnlTransporte != null)
            {
                pnlTransporte.Visible = !ocultarExtras;
            }

            if (pnlAlergia != null)
            {
                pnlAlergia.Visible = !ocultarExtras;
            }

            if (rfvTransporte != null)
            {
                rfvTransporte.Enabled = !ocultarExtras;
            }

            if (itemContainer != null)
            {
                string baseClass = "integrante-item";
                if (string.Equals(rblAsistencia?.SelectedValue, "Si", StringComparison.OrdinalIgnoreCase))
                {
                    itemContainer.Attributes["class"] = baseClass + " asistencia-si";
                }
                else if (string.Equals(rblAsistencia?.SelectedValue, "No", StringComparison.OrdinalIgnoreCase))
                {
                    itemContainer.Attributes["class"] = baseClass + " asistencia-no";
                }
                else
                {
                    itemContainer.Attributes["class"] = baseClass;
                }
            }

            if (ocultarExtras)
            {
                rblTransporte?.ClearSelection();
                if (txtAlergia != null)
                {
                    txtAlergia.Text = string.Empty;
                }
            }
        }

        // Manejador para enviar comentarios Y guardar todos los cambios
        protected void btnEnviarComentario_Click(object sender, EventArgs e)
        {
            if (InvitacionCerrada)
            {
                MostrarMensajeError("La invitación está cerrada.");
                return;
            }

            if (!Page.IsValid)
            {
                pnlMensajeError.Visible = false;
                pnlMensajeExito.Visible = false;
                return;
            }

            try
            {
                int cambiosGuardados = 0;

                // Extraer el comentario antes de entrar al bucle para tenerlo disponible
                string comentario = txtComentario.Text.Trim();
                StringBuilder detalleIntegrantes = new StringBuilder();

                // Valor común de práctica (Si/No) tomado del radio superior (puede ser null)
                string practicaGlobal = rblPractica.SelectedValue;

                // Fecha y hora de última modificación con formato dd/MM/yyyy HH:mm:ss (ej: 05/03/2026 14:00:00)
                string fechaUltimaModificacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    // Incluimos Fecha_ult_modificacion en el UPDATE
                    string query = "UPDATE Equipo_participa SET p_asistencia = ?, p_transporte = ?, p_alergia = ?, p_comentario = ?, p_practica = ?, Fecha_ult_modificacion = ? WHERE p_contador = ?";

                    // 1. Guardar todos los cambios de los integrantes junto con el comentario
                    foreach (RepeaterItem item in rptIntegrantes.Items)
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            // Encontrar los nuevos controles
                            RadioButtonList rblAsistencia = (RadioButtonList)item.FindControl("rblAsistencia");
                            RadioButtonList rblTransporte = (RadioButtonList)item.FindControl("rblTransporte");
                            TextBox txtAlergia = (TextBox)item.FindControl("txtAlergia");
                            HiddenField hdnContador = (HiddenField)item.FindControl("hdnContador");
                            var lblEditNombre = (Label)item.FindControl("lblEditNombre");

                            if (rblAsistencia != null && rblTransporte != null && txtAlergia != null && hdnContador != null)
                            {
                                int id = Convert.ToInt32(hdnContador.Value);

                                // Extraer los strings en lugar de booleanos ("Si" o "No")
                                string asistencia = rblAsistencia.SelectedValue;
                                string transporte = rblTransporte.SelectedValue;
                                string alergia = txtAlergia.Text.Trim();

                                if (string.Equals(asistencia, "No", StringComparison.OrdinalIgnoreCase))
                                {
                                    transporte = null;
                                    alergia = null;
                                }
                                string nombreIntegrante = lblEditNombre != null ? lblEditNombre.Text.Trim() : string.Empty;

                                if (!string.IsNullOrEmpty(nombreIntegrante))
                                {
                                    string asistenciaTexto = asistencia == "Si" ? "Sí" : asistencia;
                                    string transporteTexto = transporte == "Si" ? "Sí" : transporte;
                                    detalleIntegrantes.AppendLine($"- {nombreIntegrante} | Asiste: {asistenciaTexto} | Transporte: {transporteTexto} | Alergia: {(string.IsNullOrEmpty(alergia) ? "(sin datos)" : alergia)}");
                                }

                                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                                {
                                    // Los parámetros en OleDb se asignan estrictamente por orden posicional (?)
                                    // Usamos DBNull.Value en caso de que lleguen vacíos
                                    cmd.Parameters.AddWithValue("@p_asistencia", string.IsNullOrEmpty(asistencia) ? (object)DBNull.Value : asistencia);
                                    cmd.Parameters.AddWithValue("@p_transporte", string.IsNullOrEmpty(transporte) ? (object)DBNull.Value : transporte);
                                    cmd.Parameters.AddWithValue("@p_alergia", string.IsNullOrEmpty(alergia) ? (object)DBNull.Value : alergia);
                                    cmd.Parameters.AddWithValue("@p_comentario", string.IsNullOrEmpty(comentario) ? (object)DBNull.Value : comentario);
                                    cmd.Parameters.AddWithValue("@p_practica", string.IsNullOrEmpty(practicaGlobal) ? (object)DBNull.Value : practicaGlobal);
                                    cmd.Parameters.AddWithValue("@Fecha_ult_modificacion", fechaUltimaModificacion);
                                    cmd.Parameters.AddWithValue("@p_contador", id);

                                    cmd.ExecuteNonQuery();
                                    cambiosGuardados++;
                                }
                            }
                        }
                    }
                } // Fin del using de la conexión (se cierra automáticamente)

                // 2. Comprobamos si había comentario para el mensaje
                string mensajeComentario = "";
                if (!string.IsNullOrEmpty(comentario))
                {
                    mensajeComentario = $" Comentario guardado: \"{comentario}\"";
                    // Vaciamos la caja de texto después de guardarlo con éxito
                    txtComentario.Text = string.Empty;
                }

                // 3. Enviar el correo notificando los cambios
                string nombreEquipo = Session["NombreEquipo"]?.ToString() ?? "Equipo Desconocido";
                EnviarCorreo(nombreEquipo, cambiosGuardados, comentario, detalleIntegrantes.ToString());

                // 4. Mostrar mensaje de éxito
                MostrarMensajeExito($"✓ {cambiosGuardados} integrante(s) actualizado(s).{mensajeComentario}");

                Response.Redirect("Agradecimiento.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar: " + ex.Message);
            }
        }

        private void EnviarCorreo(string nombreEquipo, int integrantesActualizados, string comentario, string detalleIntegrantes)
        {
            try
            {
                /*
                 
                    Aquí hay que quitar mi correo y poner el que se vaya a usar para enviar las notificaciones.
                    También hay que cambiar el correo del recptor y modificar la fromPassword ya que solo funciona
                    para mi correo f.oliverosafonso2@gmail.com.
                    Funciona bien, de esta forma solo enviamos en el correo que se han realizado cambios y también 
                    el comentario que se escriba, pero sin incluir los datos personales ni las opciones de coche ni asiste.
                    Hay que hacer que también ponga la fecha de modificación (supongo)
                    
                 */
                var fromAddress = new MailAddress("f.oliverosafonso2@gmail.com", "Torneo de Golf");
                var toAddress = new MailAddress("eugenio@mesia.es");

                const string fromPassword = "wtittjtuldcyorfb";
                string subject = $"Cambios en la base de datos - Equipo: {nombreEquipo}";

                string body = $"Se han actualizado los datos de {integrantesActualizados} integrante(s) pertenecientes al equipo '{nombreEquipo}'.\n" +
                    $"El equipo quedó así:\n";

                if (!string.IsNullOrWhiteSpace(detalleIntegrantes))
                {
                    body += detalleIntegrantes + "\n \n";
                }

                if (!string.IsNullOrEmpty(comentario))
                {
                    body += $"\nComentario adjunto:\n{comentario}";
                }

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000,
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                LogError("Error al intentar enviar el correo", ex);
                System.Diagnostics.Debug.WriteLine($"Error al intentar enviar el correo: {ex.Message}");
            }
        }

        private void LogError(string mensaje, Exception ex)
        {
            try
            {
                string dir = Server.MapPath("~/App_Data");
                Directory.CreateDirectory(dir);

                string ruta = Path.Combine(dir, "errores.log");
                string linea = $"{DateTime.UtcNow:O} | {mensaje} | {ex}\r\n";
                File.AppendAllText(ruta, linea, Encoding.UTF8);
            }
            catch
            {
                // Evitar romper la app si el log falla
            }
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

        private bool InvitacionCerrada
        {
            get
            {
                object valor = ViewState["InvitacionCerrada"];
                return valor != null && (bool)valor;
            }
            set
            {
                ViewState["InvitacionCerrada"] = value;
            }
        }

        private bool EsInvitacionCerrada(object valorCampo)
        {
            if (valorCampo == null || valorCampo == DBNull.Value)
                return false;

            bool valorBool;
            if (bool.TryParse(valorCampo.ToString(), out valorBool))
                return valorBool;

            int valorInt;
            if (int.TryParse(valorCampo.ToString(), out valorInt))
                return valorInt == 1;

            return false;
        }

        private void BloquearCamposInvitacion()
        {
            foreach (RepeaterItem item in rptIntegrantes.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    RadioButtonList rblAsistencia = item.FindControl("rblAsistencia") as RadioButtonList;
                    RadioButtonList rblTransporte = item.FindControl("rblTransporte") as RadioButtonList;
                    TextBox txtAlergia = item.FindControl("txtAlergia") as TextBox;
                    RequiredFieldValidator rfvAsistencia = item.FindControl("rfvAsistencia") as RequiredFieldValidator;
                    RequiredFieldValidator rfvTransporte = item.FindControl("rfvTransporte") as RequiredFieldValidator;

                    if (rblAsistencia != null) rblAsistencia.Enabled = false;
                    if (rblTransporte != null) rblTransporte.Enabled = false;
                    if (txtAlergia != null) txtAlergia.ReadOnly = true;
                    if (rfvAsistencia != null) rfvAsistencia.Enabled = false;
                    if (rfvTransporte != null) rfvTransporte.Enabled = false;
                }
            }

            if (rblPractica != null) rblPractica.Enabled = false;
            if (txtComentario != null) txtComentario.ReadOnly = true;
            if (btnEnviarComentario != null) btnEnviarComentario.Enabled = false;
        }
    }
}