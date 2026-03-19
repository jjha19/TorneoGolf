using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

namespace WebApplication2
{
    public partial class Torneos : System.Web.UI.Page
    {
        // Reusamos la misma lógica de conexión de tu proyecto
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
            // Protección de ruta: Si no hay usuario logueado, lo devolvemos al Login
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarTorneosUsuario();
            }
        }

        private void CargarTorneosUsuario()
        {
            try
            {
                DataTable dt = new DataTable();
                string usuarioLogueado = Session["Usuario"]?.ToString() ?? "";

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    // Añadimos t_codigo a la consulta para poder mandarlo por la URL
                    string query = "SELECT t_codigo, t_nombre FROM Torneos WHERE t_usuario = ?"; 
                    
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", usuarioLogueado);

                        conn.Open();
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    rptTorneos.DataSource = dt;
                    rptTorneos.DataBind();
                    pnlNoData.Visible = false;
                }
                else
                {
                    rptTorneos.DataSource = null;
                    rptTorneos.DataBind();
                    pnlNoData.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al cargar tus torneos: " + ex.Message);
            }
        }

        private void MostrarMensajeError(string mensaje)
        {
            lblMensajeError.Text = mensaje;
            pnlMensajeError.Visible = true;
        }
    }
}