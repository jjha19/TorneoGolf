using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

namespace WebApplication2
{
    public partial class ListaEquipos : Page
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
                CargarEquipos();
            }
        }

        private void CargarEquipos()
        {
            string torneoCodigo = (Request.QueryString["torneo"] ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se ha indicado un torneo válido.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string queryNombreTorneo = "SELECT t_nombre FROM Torneos WHERE t_codigo = ?";
                    using (OleDbCommand cmdNombre = new OleDbCommand(queryNombreTorneo, conn))
                    {
                        cmdNombre.Parameters.AddWithValue("?", torneoCodigo);
                        object nombre = cmdNombre.ExecuteScalar();
                        lblTorneo.Text = nombre == null || nombre == DBNull.Value
                            ? "Código: " + torneoCodigo
                            : nombre.ToString() + " (" + torneoCodigo + ")";
                    }

                    DataTable dt = new DataTable();
                    string queryEquipos = @"SELECT e_contador, e_codigo, e_nombre, e_movil, e_contacto
                                            FROM Equipo
                                            WHERE e_torneo = ?
                                            ORDER BY e_nombre ASC, e_codigo ASC";

                    using (OleDbCommand cmdEquipos = new OleDbCommand(queryEquipos, conn))
                    {
                        cmdEquipos.Parameters.AddWithValue("?", torneoCodigo);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdEquipos);
                        adapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        rptEquipos.DataSource = dt;
                        rptEquipos.DataBind();
                        pnlNoData.Visible = false;
                    }
                    else
                    {
                        rptEquipos.DataSource = null;
                        rptEquipos.DataBind();
                        pnlNoData.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al cargar equipos: " + ex.Message);
            }
        }

        private void MostrarMensajeError(string mensaje)
        {
            lblMensajeError.Text = mensaje;
            pnlMensajeError.Visible = true;
        }
    }
}