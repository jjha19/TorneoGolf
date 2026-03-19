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
                // Login exitoso, nos vamos a la web en la que se ven los participantes del grupo
                Session["Usuario"] = usuario;
                Response.Redirect("Torneos.aspx");
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
            string dbPath = Server.MapPath("~/Database/Torneo.accdb");

            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                "Data Source=" + dbPath + ";" +
                                "Persist Security Info=False;";


            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Consulta para obtener el nombre del torneo y el móvil
                    string query = "SELECT t_nombre, t_movil FROM Torneos WHERE t_usuario = ? AND t_contra = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", usuario);
                        cmd.Parameters.AddWithValue("?", password);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Guardar el nombre del torneo y el móvil en la sesión
                                Session["NombreTorneo"] = reader["t_nombre"].ToString().Trim();
                                string movil = reader["t_movil"].ToString().Trim();
                                Session["Movil"] = movil;

                                // Buscar el nombre del equipo en la tabla Equipo usando el móvil
                                string queryEquipo = "SELECT e_nombre FROM Equipo WHERE e_movil = ?";
                                using (OleDbCommand cmdEquipo = new OleDbCommand(queryEquipo, conn))
                                {
                                    cmdEquipo.Parameters.AddWithValue("?", movil);
                                    
                                    using (OleDbDataReader readerEquipo = cmdEquipo.ExecuteReader())
                                    {
                                        if (readerEquipo.Read())
                                        {
                                            Session["NombreEquipo"] = readerEquipo["e_nombre"].ToString().Trim();
                                        }
                                        else
                                        {
                                            // Si no se encuentra el equipo, guardar un valor por defecto
                                            Session["NombreEquipo"] = "Equipo Sin Nombre";
                                        }
                                    }
                                }

                                return true;
                            }
                            return false;
                        }
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