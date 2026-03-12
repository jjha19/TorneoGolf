using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
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
            // Verificar si el usuario está logueado
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                InicializarPagina();
                CargarIntegrantes();
            }
        }

        private void InicializarPagina()
        {
            // Código de equipo fijo (más adelante se obtendrá por parámetro de URL)
            string codigoEquipo = "AB124";

            // Obtener el nombre del equipo desde la BD
            string nombreEquipo = ObtenerNombreEquipo(codigoEquipo);

            // Obtener el nombre del torneo (puede venir de Session o ser fijo)
            string nombreTorneo = Session["NombreTorneo"]?.ToString() ?? "Torneo de Golf 2026";

            // DEBUG: Mostrar qué nombre de torneo estamos usando
            System.Diagnostics.Debug.WriteLine($"Buscando torneo: '{nombreTorneo}'");

            // Obtener el comentario del torneo desde la BD usando el nombre del torneo
            string comentarioTorneo = ObtenerComentarioTorneo(nombreTorneo);

            // DEBUG: Mostrar qué comentario obtuvimos
            System.Diagnostics.Debug.WriteLine($"Comentario obtenido: '{comentarioTorneo}'");

            // Asignar valores a los labels del título
            lblTituloTorneo.Text = nombreTorneo;
            lblTituloEquipo.Text = nombreEquipo;
            lblComentarioTorneo.Text = comentarioTorneo;
        }

        private string ObtenerComentarioTorneo(string nombreTorneo)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Primero intentar buscar por nombre exacto
                    string query = "SELECT t_comentario FROM Torneos WHERE t_nombre = ?";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@t_nombre", nombreTorneo);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                    {
                        return resultado.ToString().Trim();
                    }

                    // Si no encuentra por nombre, intentar obtener el primer registro
                    query = "SELECT TOP 1 t_comentario FROM Torneos";
                    cmd = new OleDbCommand(query, conn);
                    resultado = cmd.ExecuteScalar();

                    if (resultado != null && !string.IsNullOrWhiteSpace(resultado.ToString()))
                    {
                        return resultado.ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar el error específico para debugging
                MostrarMensajeError($"Error al obtener el comentario del torneo: {ex.Message}");
            }

            return "Sin comentario disponible"; // Retornar mensaje informativo
        }

        private string ObtenerNombreEquipo(string codigoEquipo)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string query = "SELECT e_nombre FROM Equipo WHERE e_codigo = ?";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@e_codigo", codigoEquipo);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        return resultado.ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al obtener el nombre del equipo: " + ex.Message);
            }

            return "Equipo Sin Nombre";
        }

        private void CargarIntegrantes()
        {
            try
            {
                DataTable dt = new DataTable();

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string query = "SELECT p_contador, p_nombre, p_asistencia, p_transporte, p_alergia FROM Equipo_participa WHERE e_codigo = ?";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@e_codigo", "AB124");

                    conn.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    adapter.Fill(dt);
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
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Editar")
            {
                MostrarModoEdicion(e.Item, true);
            }
            else if (e.CommandName == "CancelarEdit")
            {
                MostrarModoEdicion(e.Item, false);
            }
            else if (e.CommandName == "GuardarEdit")
            {
                GuardarEdicionInline(e.Item, id);
            }
        }

        private void MostrarModoEdicion(RepeaterItem item, bool editar)
        {
            Panel pnlView = (Panel)item.FindControl("pnlView");
            Panel pnlEdit = (Panel)item.FindControl("pnlEdit");

            if (pnlView != null && pnlEdit != null)
            {
                pnlView.Visible = !editar;
                pnlEdit.Visible = editar;
            }
        }

        private void GuardarEdicionInline(RepeaterItem item, int id)
        {
            try
            {
                // Obtener valores de los controles
                CheckBox chkEditAsistencia = (CheckBox)item.FindControl("chkEditAsistencia");
                CheckBox chkEditTransporte = (CheckBox)item.FindControl("chkEditTransporte");

                // Actualizar en la BD directamente
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string query = "UPDATE Equipo_participa SET p_asistencia = ?, p_transporte = ? WHERE p_contador = ?";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@p_asistencia", chkEditAsistencia.Checked);
                    cmd.Parameters.AddWithValue("@p_transporte", chkEditTransporte.Checked);
                    cmd.Parameters.AddWithValue("@p_contador", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MostrarMensajeExito("Cambios guardados exitosamente.");

                // Volver a modo vista
                MostrarModoEdicion(item, false);
                CargarIntegrantes();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar la edición: " + ex.Message);
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

        // Manejador para enviar comentarios
        protected void btnEnviarComentario_Click(object sender, EventArgs e)
        {
            try
            {
                string comentario = txtComentario.Text.Trim();

                if (string.IsNullOrEmpty(comentario))
                {
                    MostrarMensajeError("Por favor, escribe un comentario antes de enviar.");
                    return;
                }

                // Aquí puedes guardar el comentario en la BD o enviarlo por email
                // Por ahora solo mostramos un mensaje de éxito
                MostrarMensajeExito($"Comentario enviado exitosamente: \"{comentario}\"");

                // Limpiar el campo de comentario
                txtComentario.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al enviar el comentario: " + ex.Message);
            }
        }
    }
}