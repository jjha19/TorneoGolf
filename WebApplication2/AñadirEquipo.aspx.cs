using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

namespace WebApplication2
{
    public partial class AñadirEquipo : Page
    {
        private string connectionString
        {
            get
            {
                string rutaDB = Server.MapPath("~/Database/Torneo.accdb");
                return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={rutaDB};Persist Security Info=False;";
            }
        }

        private string CodigoTorneo
        {
            get { return ViewState["CodigoTorneo"] as string ?? string.Empty; }
            set { ViewState["CodigoTorneo"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string torneoCodigo = (Request.QueryString["torneo"] ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se ha indicado el torneo.");
                btnGuardarEquipo.Enabled = false;
                return;
            }

            if (!IsPostBack)
            {
                if (!TorneoPerteneceAUsuario(torneoCodigo))
                {
                    MostrarMensajeError("No tienes permisos sobre ese torneo.");
                    btnGuardarEquipo.Enabled = false;
                    return;
                }

                CodigoTorneo = torneoCodigo;
                lblTorneoActual.Text = "Torneo: " + torneoCodigo;
            }
        }

        private bool TorneoPerteneceAUsuario(string torneoCodigo)
        {
            string usuarioLogueado = Session["Usuario"] == null ? string.Empty : Session["Usuario"].ToString();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Torneos WHERE t_codigo = ? AND t_usuario = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", torneoCodigo);
                    cmd.Parameters.AddWithValue("?", usuarioLogueado);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();
                    int total = 0;
                    if (resultado != null && resultado != DBNull.Value)
                    {
                        int.TryParse(resultado.ToString(), out total);
                    }

                    return total > 0;
                }
            }
        }

        protected void btnGuardarEquipo_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (string.IsNullOrWhiteSpace(CodigoTorneo))
            {
                MostrarMensajeError("Código de torneo no válido.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string query = @"INSERT INTO Equipo (e_codigo, e_nombre, e_movil, e_contacto, e_torneo)
                                     VALUES (?, ?, ?, ?, ?)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", txtCodigoEquipo.Text.Trim().ToUpper());
                        cmd.Parameters.AddWithValue("?", txtNombreEquipo.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtMovilEquipo.Text) ? (object)DBNull.Value : txtMovilEquipo.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtContactoEquipo.Text) ? (object)DBNull.Value : txtContactoEquipo.Text.Trim());
                        cmd.Parameters.AddWithValue("?", CodigoTorneo);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MostrarMensajeExito("Equipo añadido correctamente.");
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar el equipo: " + ex.Message);
            }
        }

        private void LimpiarFormulario()
        {
            txtCodigoEquipo.Text = string.Empty;
            txtNombreEquipo.Text = string.Empty;
            txtMovilEquipo.Text = string.Empty;
            txtContactoEquipo.Text = string.Empty;
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