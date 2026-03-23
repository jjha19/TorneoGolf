using System;
using System.Data;
using System.Data.OleDb;
using System.Text;
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

        protected void btnDescargar_Click(object sender, EventArgs e)
        {
            string torneoCodigo = Request.QueryString["torneo"];

            if (string.IsNullOrEmpty(torneoCodigo))
            {
                MostrarMensajeError("No se ha seleccionado ningún torneo válido.");
                return;
            }

            DataTable dt = new DataTable();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string queryParticipantes = "SELECT p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_comentario, p_practica " +
                                                "FROM Equipo_participa WHERE p_torneo = ?";

                    using (OleDbCommand cmdPart = new OleDbCommand(queryParticipantes, conn))
                    {
                        cmdPart.Parameters.AddWithValue("?", torneoCodigo);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdPart);
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al descargar los participantes: " + ex.Message);
                return;
            }

            if (dt.Rows.Count == 0)
            {
                MostrarMensajeError("No hay participantes para descargar.");
                return;
            }

            string nombreArchivo = $"participantes_{torneoCodigo}.csv";
            string csv = ConvertirDataTableACsv(dt);

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");
            Response.Write("\uFEFF");
            Response.Write(csv);
            Response.Flush();
            Response.End();
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
                    
                    // AÑADIDOS LOS NUEVOS CAMPOS AQUÍ: p_asistencia, p_transporte, p_alergia, p_comentario, Y p_practica
                    string queryParticipantes = "SELECT p_nombre, p_apellido, p_movi, p_asistencia, p_transporte, p_alergia, p_comentario, p_practica " +
                                                "FROM Equipo_participa WHERE p_torneo = ?";
                    
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

        private static string ConvertirDataTableACsv(DataTable dt)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Nombre,Apellido,Movil,Asistencia,Transporte,Alergia,Comentario,Practica");

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine(string.Join(",", new[]
                {
                    EscaparCsv(row["p_nombre"]),
                    EscaparCsv(row["p_apellido"]),
                    EscaparCsv(row["p_movi"]),
                    EscaparCsv(row["p_asistencia"]),
                    EscaparCsv(row["p_transporte"]),
                    EscaparCsv(row["p_alergia"]),
                    EscaparCsv(row["p_comentario"]),
                    EscaparCsv(row["p_practica"])
                }));
            }

            return sb.ToString();
        }

        private static string EscaparCsv(object valor)
        {
            if (valor == null || valor == DBNull.Value)
            {
                return "";
            }

            string texto = valor.ToString();
            if (texto.Contains("\"") || texto.Contains(",") || texto.Contains("\n") || texto.Contains("\r"))
            {
                texto = texto.Replace("\"", "\"\"");
                return $"\"{texto}\"";
            }

            return texto;
        }
    }
}