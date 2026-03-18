using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
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
            if (!IsPostBack)
            {
                InicializarPagina();
                CargarIntegrantes();
            }
        }

        private void InicializarPagina()
        {
            string nombreTorneo = "Torneo no encontrado";
            string comentarioTorneo = "Sin comentario disponible";
            string nombreEquipo = "Equipo no encontrado";
            string codigoEquipo = "AB124"; // Valor por defecto
            string torneoVinculado = "";        

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

                    // 1. Buscar primero en la tabla Equipo usando el e_contador
                    string queryEquipo = "SELECT e_nombre, e_codigo, e_torneo FROM Equipo WHERE e_contador = ?";
                    using (OleDbCommand cmdEquipo = new OleDbCommand(queryEquipo, conn))
                    {
                        cmdEquipo.Parameters.AddWithValue("?", equipoContador);
                        using (OleDbDataReader readerEquipo = cmdEquipo.ExecuteReader())
                        {
                            if (readerEquipo.Read())
                            {
                                nombreEquipo = readerEquipo["e_nombre"].ToString().Trim();
                                codigoEquipo = readerEquipo["e_codigo"]?.ToString().Trim();
                                torneoVinculado = readerEquipo["e_torneo"]?.ToString().Trim();
                                
                                Session["NombreEquipo"] = nombreEquipo;
                                Session["CodigoEquipo"] = codigoEquipo; // Importante para CargarIntegrantes()
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"No existe el equipo con e_contador = {equipoContador}");
                            }
                        }
                    }

                    // 2. Si encontramos el torneo vinculado (código del torneo), buscamos su información en la tabla Torneos
                    if (!string.IsNullOrEmpty(torneoVinculado))
                    {
                        // IMPORTANTE: Buscamos por t_codigo usando el e_torneo
                        string queryTorneo = "SELECT t_nombre, t_comen FROM Torneos WHERE t_codigo = ?";
                        using (OleDbCommand cmdTorneo = new OleDbCommand(queryTorneo, conn))
                        {
                            cmdTorneo.Parameters.AddWithValue("?", torneoVinculado);
                            using (OleDbDataReader readerTorneo = cmdTorneo.ExecuteReader())
                            {
                                if (readerTorneo.Read())
                                {
                                    // Asignamos el t_nombre a la variable que se mostrará
                                    nombreTorneo = readerTorneo["t_nombre"].ToString().Trim();
                                    comentarioTorneo = readerTorneo["t_comen"]?.ToString().Trim();
                                    
                                    // Guardar en sesión para usarlo en otras partes si es necesario
                                    Session["NombreTorneo"] = nombreTorneo;
                                }
                                else
                                {
                                    // Fallback: Si no está en la tabla Torneos
                                    nombreTorneo = "Código no encontrado: " + torneoVinculado;
                                    System.Diagnostics.Debug.WriteLine($"No se encontró información extra para el torneo con código: {torneoVinculado}");
                                }
                            }
                        }
                    }
                }
            }           
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al inicializar la página: {ex.Message}");
            }

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
                    string query = "SELECT p_contador, p_nombre, p_asistencia, p_transporte, p_alergia, p_practica FROM Equipo_participa WHERE e_codigo = ?";
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

        protected void rptIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {       
            // Ya no se usa porque eliminamos los botones individuales
        }

        // Manejador para enviar comentarios Y guardar todos los cambios
        protected void btnEnviarComentario_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                MostrarMensajeError("Por favor, complete todos los campos obligatorios (Asistencia y Transporte) de todos los integrantes.");
                return;
            }

            try
            {
                int cambiosGuardados = 0;
                
                // Extraer el comentario antes de entrar al bucle para tenerlo disponible
                string comentario = txtComentario.Text.Trim();
                StringBuilder detalleIntegrantes = new StringBuilder();

                // Abrir la conexión una sola vez fuera del bucle para mejorar el rendimiento
                // Valor común de práctica (Si/No) tomado del radio superior (puede ser null)
                string practicaGlobal = rblPractica.SelectedValue;

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    // Modificamos la consulta para incluir p_comentario y p_practica
                    string query = "UPDATE Equipo_participa SET p_asistencia = ?, p_transporte = ?, p_alergia = ?, p_comentario = ?, p_practica = ? WHERE p_contador = ?";

                    // 1. Guardar todos los cambios de los integrantes junto con el comentario
                    foreach (RepeaterItem item in rptIntegrantes.Items)
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            // Encontrar los nuevos controles
                            RadioButtonList rblAsistencia = (RadioButtonList)item.FindControl("rblAsistencia");
                            RadioButtonList rblTransporte = (RadioButtonList)item.FindControl("rblTransporte");
                            TextBox txtAlergia = (TextBox)item.FindControl("txtAlergia");
                            Label lblEditNombre = (Label)item.FindControl("lblEditNombre");
                            HiddenField hdnContador = (HiddenField)item.FindControl("hdnContador");

                            if (rblAsistencia != null && rblTransporte != null && txtAlergia != null && hdnContador != null)
                            {
                                int id = Convert.ToInt32(hdnContador.Value);
                                
                                // Extraer los strings en lugar de booleanos ("Si" o "No")
                                string asistencia = rblAsistencia.SelectedValue;
                                string transporte = rblTransporte.SelectedValue;
                                string alergia = txtAlergia.Text.Trim();
                                string nombreIntegrante = lblEditNombre?.Text.Trim() ?? string.Empty;

                                if (!string.IsNullOrEmpty(nombreIntegrante))
                                {
                                    string asistenciaTexto = asistencia == "Si" ? "Sí" : asistencia;
                                    string transporteTexto = transporte == "Si" ? "Sí" : transporte;
                                    detalleIntegrantes.AppendLine($"- {nombreIntegrante} | Alergia: {(string.IsNullOrEmpty(alergia) ? "(sin datos)" : alergia)} | Asiste: {asistenciaTexto} | Transporte: {transporteTexto}");
                                }

                                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                                {
                                    // Los parámetros en OleDb se asignan estrictamente por orden posicional (?
                                    // Usamos DBNull.Value en caso de que lleguen vacíos
                                    cmd.Parameters.AddWithValue("@p_asistencia", string.IsNullOrEmpty(asistencia) ? (object)DBNull.Value : asistencia);
                                    cmd.Parameters.AddWithValue("@p_transporte", string.IsNullOrEmpty(transporte) ? (object)DBNull.Value : transporte);
                                    cmd.Parameters.AddWithValue("@p_alergia", string.IsNullOrEmpty(alergia) ? (object)DBNull.Value : alergia);
                                    cmd.Parameters.AddWithValue("@p_comentario", string.IsNullOrEmpty(comentario) ? (object)DBNull.Value : comentario);
                                    cmd.Parameters.AddWithValue("@p_practica", string.IsNullOrEmpty(practicaGlobal) ? (object)DBNull.Value : practicaGlobal);
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
                
                // Refrescamos los datos para mostrarlos actualizados
                CargarIntegrantes();
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
                 
                 */
                var fromAddress = new MailAddress("f.oliverosafonso2@gmail.com", "Torneo de Golf");
                var toAddress = new MailAddress("juanjose.spider@gmail.com");
                
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
                // Registramos el error de correo en consola de depuración para no romper la ejecución de la app
                // Si el correo falla, los datos seguirán estando guardados en base de datos.
                System.Diagnostics.Debug.WriteLine($"Error al intentar enviar el correo: {ex.Message}");
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
    }
}