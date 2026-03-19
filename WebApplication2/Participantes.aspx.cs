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
            // Rescatamos el ?torneo= de la URL enviado por la pantalla anterior
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

                    // 1. (Opcional) Obtener el nombre del torneo para ponerlo en el título
                    string queryTorneo = "SELECT t_nombre FROM Torneos WHERE t_codigo = ?";
                    using (OleDbCommand cmdTorneo = new OleDbCommand(queryTorneo, conn))
                    {
                        cmdTorneo.Parameters.AddWithValue("?", torneoCodigo);
                        object nombreResultado = cmdTorneo.ExecuteScalar(); // Coge solo un dato
                        if (nombreResultado != null)
                        {
                            lblNombreTorneo.Text = nombreResultado.ToString();
                        }
                        else
                        {
                            lblNombreTorneo.Text = "Torneo Desconocido";
                        }
                    }

                    // 2. Extraer a los participantes de la tabla Equipo_participa donde p_torneo es el código
                    DataTable dt = new DataTable();
                    string queryParticipantes = "SELECT p_nombre, p_apellido, p_movi FROM Equipo_participa WHERE p_torneo = ?";
                    
                    using (OleDbCommand cmdPart = new OleDbCommand(queryParticipantes, conn))
                    {
                        cmdPart.Parameters.AddWithValue("?", torneoCodigo);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdPart);
                        adapter.Fill(dt);
                    }

                    // 3. Emparejar los datos visualmente
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

        private void MostrarMensajeError(string mensaje)
        {
            lblMensajeError.Text = mensaje;
            pnlMensajeError.Visible = true;
            pnlNoData.Visible = false;
        }
    }
}