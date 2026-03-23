using System;
using System.Data;
using System.Data.OleDb;
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
                    string queryParticipantes = "SELECT p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_comentario, p_practica " +
                                                "FROM Equipo_participa WHERE p_torneo = ?";
                    
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

                    // Consulta de inserción para el nuevo participante
                    string query = @"INSERT INTO Equipo_participa 
                                     (p_torneo, p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_practica, p_comentario) 
                                     VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        // IMPORTANTE OLEDB: El orden de los parámetros DEBE coincidir con los interrogantes en exactitud.
                        cmd.Parameters.AddWithValue("@p_torneo", torneoCodigo);
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
    }
}