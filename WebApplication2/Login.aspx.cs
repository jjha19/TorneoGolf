using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Configuration;

namespace WebApplication2
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Validamos que los campos de usuario y contraseña no estén vacíos
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMensaje.Text = "Por favor, ingrese usuario y contraseña";
                lblMensaje.Visible = true;
                return;
            }

            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            // Validamos usuario y contraseña contra la base de datos
            if (ValidarUsuario(usuario, password))
            {
                // Login exitoso
                Session["Usuario"] = usuario;
                Response.Redirect("Principal.aspx");
            }
            else
            {
                // Si falla el login, mostramos un mensaje de error
                lblMensaje.Text = "Usuario o contraseña incorrectos";
                lblMensaje.Visible = true;
                txtPassword.Text = string.Empty;
            }
        }

        private bool ValidarUsuario(string usuario, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TorneoConnection"].ConnectionString;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta con parámetros para evitar inyección SQL
                    string query = "SELECT COUNT(*) FROM Torneos WHERE t_usuario = ? AND t_contra = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", usuario);
                        cmd.Parameters.AddWithValue("?", password);

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    // Registrar el error (en producción usar un sistema de logs adecuado)
                    lblMensaje.Text = "Error al conectar con la base de datos: " + ex.Message;
                    lblMensaje.Visible = true;
                    return false;
                }
            }
        }
    }
}