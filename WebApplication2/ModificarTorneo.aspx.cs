using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

namespace WebApplication2
{
    public partial class ModificarTorneo : Page
    {
        private string connectionString
        {
            get
            {
                string rutaDB = Server.MapPath("~/Database/Torneo.accdb");
                return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={rutaDB};Persist Security Info=False;";
            }
        }

        private int TorneoContador
        {
            get
            {
                object valor = ViewState["TorneoContador"];
                return valor == null ? 0 : (int)valor;
            }
            set
            {
                ViewState["TorneoContador"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int torneoId;
            if (!int.TryParse(Request.QueryString["torneoId"], out torneoId) || torneoId <= 0)
            {
                MostrarMensajeError("No se ha indicado correctamente el torneo a modificar.");
                btnGuardar.Enabled = false;
                return;
            }

            if (!IsPostBack)
            {
                TorneoContador = torneoId;
                CargarTorneo(torneoId);
            }
        }

        private void CargarTorneo(int torneoId)
        {
            string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : string.Empty;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string query = @"SELECT t_codigo, t_nombre, t_contacto, t_movil, t_fecha, t_usuario, t_contra, t_comen, texto_ws
                                     FROM Torneos
                                     WHERE t_contador = ? AND t_usuario = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", torneoId);
                        cmd.Parameters.AddWithValue("?", usuarioLogueado);

                        conn.Open();
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtCodigo.Text = reader["t_codigo"] == DBNull.Value ? "" : reader["t_codigo"].ToString();
                                txtNombre.Text = reader["t_nombre"] == DBNull.Value ? "" : reader["t_nombre"].ToString();
                                txtContacto.Text = reader["t_contacto"] == DBNull.Value ? "" : reader["t_contacto"].ToString();
                                txtMovil.Text = reader["t_movil"] == DBNull.Value ? "" : reader["t_movil"].ToString();
                                txtFecha.Text = reader["t_fecha"] == DBNull.Value ? "" : reader["t_fecha"].ToString();
                                txtUsuario.Text = reader["t_usuario"] == DBNull.Value ? "" : reader["t_usuario"].ToString();
                                txtContrasena.Text = reader["t_contra"] == DBNull.Value ? "" : reader["t_contra"].ToString();
                                txtComentario.Text = reader["t_comen"] == DBNull.Value ? "" : reader["t_comen"].ToString();
                                txtTextoWs.Text = reader["texto_ws"] == DBNull.Value ? "" : reader["texto_ws"].ToString();
                            }
                            else
                            {
                                MostrarMensajeError("No se encontró el torneo o no tienes permisos para modificarlo.");
                                btnGuardar.Enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al cargar el torneo: " + ex.Message);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (TorneoContador <= 0)
            {
                MostrarMensajeError("Identificador de torneo no válido.");
                return;
            }

            string usuarioLogueado = Session["Usuario"] != null ? Session["Usuario"].ToString() : string.Empty;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string query = @"UPDATE Torneos
                                     SET t_codigo = ?, t_nombre = ?, t_contacto = ?, t_movil = ?, t_fecha = ?, t_usuario = ?, t_contra = ?, t_comen = ?, texto_ws = ?
                                     WHERE t_contador = ? AND t_usuario = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtCodigo.Text) ? (object)DBNull.Value : txtCodigo.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtNombre.Text) ? (object)DBNull.Value : txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtContacto.Text) ? (object)DBNull.Value : txtContacto.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtMovil.Text) ? (object)DBNull.Value : txtMovil.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtFecha.Text) ? (object)DBNull.Value : txtFecha.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtUsuario.Text) ? (object)DBNull.Value : txtUsuario.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtContrasena.Text) ? (object)DBNull.Value : txtContrasena.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtComentario.Text) ? (object)DBNull.Value : txtComentario.Text.Trim());
                        cmd.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(txtTextoWs.Text) ? (object)DBNull.Value : txtTextoWs.Text.Trim());

                        cmd.Parameters.AddWithValue("?", TorneoContador);
                        cmd.Parameters.AddWithValue("?", usuarioLogueado);

                        conn.Open();
                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                        {
                            MostrarMensajeExito("Torneo actualizado correctamente.");
                        }
                        else
                        {
                            MostrarMensajeError("No se pudo actualizar el torneo.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar cambios: " + ex.Message);
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