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
                    string query = "SELECT t_comen FROM Torneos WHERE t_nombre = ?";
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
            // Ya no se usa porque eliminamos los botones individuales
        }

        // Manejador para enviar comentarios Y guardar todos los cambios
        protected void btnEnviarComentario_Click(object sender, EventArgs e)
        {
            try
            {
                int cambiosGuardados = 0;

                // 1. Guardar todos los cambios de los integrantes
                foreach (RepeaterItem item in rptIntegrantes.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        CheckBox chkAsistencia = (CheckBox)item.FindControl("chkAsistencia");
                        CheckBox chkTransporte = (CheckBox)item.FindControl("chkTransporte");
                        TextBox txtAlergia = (TextBox)item.FindControl("txtAlergia");
                        HiddenField hdnContador = (HiddenField)item.FindControl("hdnContador");

                        if (chkAsistencia != null && chkTransporte != null && txtAlergia != null && hdnContador != null)
                        {
                            int id = Convert.ToInt32(hdnContador.Value);

                            using (OleDbConnection conn = new OleDbConnection(connectionString))
                            {
                                string query = "UPDATE Equipo_participa SET p_asistencia = ?, p_transporte = ?, p_alergia = ? WHERE p_contador = ?";
                                OleDbCommand cmd = new OleDbCommand(query, conn);
                                cmd.Parameters.AddWithValue("@p_asistencia", chkAsistencia.Checked);
                                cmd.Parameters.AddWithValue("@p_transporte", chkTransporte.Checked);
                                cmd.Parameters.AddWithValue("@p_alergia", txtAlergia.Text.Trim());
                                cmd.Parameters.AddWithValue("@p_contador", id);

                                conn.Open();
                                cmd.ExecuteNonQuery();
                                cambiosGuardados++;
                            }
                        }
                    }
                }

                // 2. Guardar el comentario (si hay algo escrito)
                string comentario = txtComentario.Text.Trim();
                string mensajeComentario = "";

                if (!string.IsNullOrEmpty(comentario))
                {
                    // Aquí puedes guardar el comentario en la BD o enviarlo por email
                    mensajeComentario = $" Comentario enviado: \"{comentario}\"";
                    txtComentario.Text = string.Empty;
                }

                // 3. Mostrar mensaje de éxito
                if (cambiosGuardados > 0 || !string.IsNullOrEmpty(comentario))
                {
                    MostrarMensajeExito($"✓ {cambiosGuardados} integrante(s) actualizado(s).{mensajeComentario}");
                    CargarIntegrantes();
                }
                else
                {
                    MostrarMensajeError("No hay cambios para guardar.");
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar: " + ex.Message);
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