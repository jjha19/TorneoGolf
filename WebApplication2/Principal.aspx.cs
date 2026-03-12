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

        // Lista temporal para cambios pendientes
        private DataTable IntegrantesPendientes
        {
            get
            {
                if (Session["IntegrantesPendientes"] == null)
                {
                    Session["IntegrantesPendientes"] = CrearTablaIntegrantes();
                }
                return (DataTable)Session["IntegrantesPendientes"];
            }
            set
            {
                Session["IntegrantesPendientes"] = value;
            }
        }

        private bool HayCambiosPendientes
        {
            get { return Session["HayCambiosPendientes"] != null && (bool)Session["HayCambiosPendientes"]; }
            set { Session["HayCambiosPendientes"] = value; }
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
            // Obtener datos del usuario desde Session
            string nombreTorneo = Session["NombreTorneo"]?.ToString() ?? "Torneo de Golf 2026";
            string nombreEquipo = Session["NombreEquipo"]?.ToString() ?? "Equipo Sin Nombre";
            string codigoEquipo = Session["CodigoEquipo"]?.ToString() ?? "EQ001";
            bool esCapitan = Session["EsCapitan"] != null && (bool)Session["EsCapitan"];

            lblNombreTorneo.Text = nombreTorneo;
            lblNombreEquipo.Text = nombreEquipo;
            lblCapitanBadge.Visible = esCapitan;

            // Mostrar/ocultar controles según rol
            btnMostrarFormulario.Visible = esCapitan;

            // Usar el código de equipo del usuario logueado
            txtCodigoEquipo.Text = codigoEquipo;
        }

        private DataTable CrearTablaIntegrantes()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("CodigoEquipo", typeof(string));
            dt.Columns.Add("Apellido", typeof(string));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Telefono", typeof(string));
            dt.Columns.Add("Transporte", typeof(string));
            dt.Columns.Add("Segundo", typeof(string));
            dt.Columns.Add("Alergias", typeof(string));
            dt.Columns.Add("Asistencia", typeof(bool));
            dt.Columns.Add("Comentario", typeof(string));
            dt.Columns.Add("EsNuevo", typeof(bool)); // Marca si es un registro nuevo
            dt.Columns.Add("EsModificado", typeof(bool)); // Marca si fue modificado
            return dt;
        }

        private void CargarIntegrantes()
        {
            try
            {
                DataTable dt = new DataTable();
                bool conexionExitosa = false;

                // Si hay cambios pendientes, mostrar los datos de la sesión
                if (HayCambiosPendientes && IntegrantesPendientes.Rows.Count > 0)
                {
                    dt = IntegrantesPendientes.Copy();
                }
                else
                {
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            string query = "SELECT * FROM Equipo_participa WHERE CodigoEquipo = ?";
                            OleDbCommand cmd = new OleDbCommand(query, conn);
                            cmd.Parameters.AddWithValue("@CodigoEquipo", txtCodigoEquipo.Text);

                            conn.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                            DataTable dtTemp = CrearTablaIntegrantes();
                            adapter.Fill(dtTemp);

                            // Marcar todos como no nuevos ni modificados
                            foreach (DataRow row in dtTemp.Rows)
                            {
                                row["EsNuevo"] = false;
                                row["EsModificado"] = false;
                            }

                            dt = dtTemp;
                            conexionExitosa = true;
                            IntegrantesPendientes = dt.Copy();
                        }
                    }
                    catch (Exception exConexion)
                    {
                        // Si falla la conexión, generar datos aleatorios
                        MostrarMensajeError("⚠️ No se pudo conectar a la base de datos. Mostrando datos de ejemplo. Error: " + exConexion.Message);
                        dt = GenerarDatosAleatorios();
                        IntegrantesPendientes = dt.Copy();
                    }
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

                // Mostrar botón de guardar cambios si hay cambios pendientes
                ActualizarBotonGuardarCambios();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al cargar integrantes: " + ex.Message);
            }
        }

        private DataTable GenerarDatosAleatorios()
        {
            DataTable dt = CrearTablaIntegrantes();

            // Listas de datos aleatorios
            string[] nombres = { "Carlos", "María", "Juan", "Ana", "Pedro", "Laura", "José", "Carmen" };
            string[] apellidos = { "García", "Martínez", "López", "Rodríguez", "Pérez", "González", "Fernández", "Sánchez" };
            string[] segundos = { "Filete de res", "Pollo asado", "Pescado a la plancha", "Pasta Alfredo", "Ensalada César", "Salmón teriyaki" };
            string[] alergias = { "Ninguna", "Mariscos", "Frutos secos", "Lácteos", "Gluten" };
            string[] transportes = { "Si", "No" };
            string[] comentarios = { "Confirmo asistencia", "Llego temprano", "Vegetariano", "" };

            Random random = new Random();

            // Generar 2 personas aleatorias
            for (int i = 1; i <= 2; i++)
            {
                DataRow row = dt.NewRow();
                row["Id"] = i;
                row["CodigoEquipo"] = txtCodigoEquipo.Text;
                row["Nombre"] = nombres[random.Next(nombres.Length)];
                row["Apellido"] = apellidos[random.Next(apellidos.Length)];
                row["Telefono"] = "555-" + random.Next(1000, 9999).ToString();
                row["Transporte"] = transportes[random.Next(transportes.Length)];
                row["Segundo"] = segundos[random.Next(segundos.Length)];
                row["Alergias"] = alergias[random.Next(alergias.Length)];
                row["Asistencia"] = random.Next(2) == 1;
                row["Comentario"] = comentarios[random.Next(comentarios.Length)];
                row["EsNuevo"] = false;
                row["EsModificado"] = false;

                dt.Rows.Add(row);
            }

            return dt;
        }

        protected void rptIntegrantes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Mostrar botones de acción solo si es capitán
                Panel pnlAcciones = (Panel)e.Item.FindControl("pnlAcciones");
                if (pnlAcciones != null)
                {
                    // TODO: Verificar si el usuario actual es capitán
                    bool esCapitan = true;
                    pnlAcciones.Visible = esCapitan;
                }

                // Configurar DropDownList de Transporte en modo edición
                DropDownList ddlEditTransporte = (DropDownList)e.Item.FindControl("ddlEditTransporte");
                if (ddlEditTransporte != null)
                {
                    DataRowView row = (DataRowView)e.Item.DataItem;
                    string transporte = row["Transporte"].ToString();
                    ddlEditTransporte.SelectedValue = transporte;
                }
            }
        }

        protected void rptIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Editar")
            {
                MostrarModoEdicion(e.Item, true);
            }
            else if (e.CommandName == "CancelarEdit")
            {
                MostrarModoEdicion(e.Item, false);
                CargarIntegrantes(); // Recargar para descartar cambios
            }
            else if (e.CommandName == "GuardarEdit")
            {
                GuardarEdicionInline(e.Item, id);
            }
            else if (e.CommandName == "Borrar")
            {
                BorrarIntegranteTemporal(id);
            }
        }

        private void MostrarModoEdicion(RepeaterItem item, bool editar)
        {
            Panel pnlView = (Panel)item.FindControl("pnlView");
            Panel pnlEdit = (Panel)item.FindControl("pnlEdit");

            if (pnlView != null && pnlEdit != null)
            {
                pnlView.Visible = !editar;
                pnlEdit.Visible = editar;
            }
        }

        private void GuardarEdicionInline(RepeaterItem item, int id)
        {
            try
            {
                // Obtener valores de los controles
                TextBox txtEditTelefono = (TextBox)item.FindControl("txtEditTelefono");
                DropDownList ddlEditTransporte = (DropDownList)item.FindControl("ddlEditTransporte");
                TextBox txtEditSegundo = (TextBox)item.FindControl("txtEditSegundo");
                TextBox txtEditAlergias = (TextBox)item.FindControl("txtEditAlergias");
                CheckBox chkEditAsistencia = (CheckBox)item.FindControl("chkEditAsistencia");
                TextBox txtEditComentario = (TextBox)item.FindControl("txtEditComentario");

                // Actualizar en la tabla temporal
                DataTable dt = IntegrantesPendientes;
                DataRow row = dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("Id") == id);

                if (row != null)
                {
                    row["Telefono"] = txtEditTelefono.Text;
                    row["Transporte"] = ddlEditTransporte.SelectedValue;
                    row["Segundo"] = txtEditSegundo.Text;
                    row["Alergias"] = txtEditAlergias.Text;
                    row["Asistencia"] = chkEditAsistencia.Checked;
                    row["Comentario"] = txtEditComentario.Text;

                    // Marcar como modificado si no es nuevo
                    if (!Convert.ToBoolean(row["EsNuevo"]))
                    {
                        row["EsModificado"] = true;
                    }

                    IntegrantesPendientes = dt;
                    HayCambiosPendientes = true;

                    MostrarMensajeExito("Cambios guardados. Haz clic en 'Guardar cambios en BD' para confirmar.");
                }

                // Volver a modo vista
                MostrarModoEdicion(item, false);
                CargarIntegrantes();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar la edición: " + ex.Message);
            }
        }

        protected void btnMostrarFormulario_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblFormTitulo.Text = "Agregar Integrante";
            pnlFormAgregar.Visible = true;
            hdnIntegranteId.Value = "0";
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlFormAgregar.Visible = false;
            LimpiarFormulario();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                int id = Convert.ToInt32(hdnIntegranteId.Value);
                DataTable dt = IntegrantesPendientes;

                if (id == 0)
                {
                    // Agregar nuevo integrante temporal
                    int nuevoId = dt.Rows.Count > 0 ? dt.AsEnumerable().Max(r => r.Field<int>("Id")) + 1 : 1;

                    DataRow row = dt.NewRow();
                    row["Id"] = nuevoId;
                    row["CodigoEquipo"] = txtCodigoEquipo.Text;
                    row["Apellido"] = txtApellido.Text;
                    row["Nombre"] = txtNombre.Text;
                    row["Telefono"] = txtTelefono.Text;
                    row["Transporte"] = ddlTransporte.SelectedValue;
                    row["Segundo"] = txtSegundo.Text;
                    row["Alergias"] = txtAlergias.Text;
                    row["Asistencia"] = chkAsistencia.Checked;
                    row["Comentario"] = txtComentario.Text;
                    row["EsNuevo"] = true;
                    row["EsModificado"] = false;

                    dt.Rows.Add(row);
                    MostrarMensajeExito("Integrante agregado a la lista. Haz clic en 'Guardar cambios' para confirmar.");
                }
                else
                {
                    // Editar integrante temporal
                    DataRow row = dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("Id") == id);
                    if (row != null)
                    {
                        row["Apellido"] = txtApellido.Text;
                        row["Nombre"] = txtNombre.Text;
                        row["Telefono"] = txtTelefono.Text;
                        row["Transporte"] = ddlTransporte.SelectedValue;
                        row["Segundo"] = txtSegundo.Text;
                        row["Alergias"] = txtAlergias.Text;
                        row["Asistencia"] = chkAsistencia.Checked;
                        row["Comentario"] = txtComentario.Text;
                        row["EsModificado"] = true;
                    }
                    MostrarMensajeExito("Integrante actualizado en la lista. Haz clic en 'Guardar cambios' para confirmar.");
                }

                IntegrantesPendientes = dt;
                HayCambiosPendientes = true;

                pnlFormAgregar.Visible = false;
                LimpiarFormulario();
                CargarIntegrantes();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al agregar el integrante: " + ex.Message);
            }
        }

        protected void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = IntegrantesPendientes;
                int cambiosRealizados = 0;

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    foreach (DataRow row in dt.Rows)
                    {
                        bool esNuevo = Convert.ToBoolean(row["EsNuevo"]);
                        bool esModificado = Convert.ToBoolean(row["EsModificado"]);

                        if (esNuevo)
                        {
                            // INSERT - Agregar nuevo integrante a la BD
                            string query = "INSERT INTO Equipo_participa (CodigoEquipo, Apellido, Nombre, Telefono, Transporte, Segundo, Alergias, Asistencia, Comentario) " +
                                          "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                            OleDbCommand cmd = new OleDbCommand(query, conn);
                            cmd.Parameters.AddWithValue("@CodigoEquipo", row["CodigoEquipo"]);
                            cmd.Parameters.AddWithValue("@Apellido", row["Apellido"]);
                            cmd.Parameters.AddWithValue("@Nombre", row["Nombre"]);
                            cmd.Parameters.AddWithValue("@Telefono", row["Telefono"]);
                            cmd.Parameters.AddWithValue("@Transporte", row["Transporte"]);
                            cmd.Parameters.AddWithValue("@Segundo", row["Segundo"]);
                            cmd.Parameters.AddWithValue("@Alergias", row["Alergias"]);
                            cmd.Parameters.AddWithValue("@Asistencia", row["Asistencia"]);
                            cmd.Parameters.AddWithValue("@Comentario", row["Comentario"]);

                            cmd.ExecuteNonQuery();
                            cambiosRealizados++;
                        }
                        else if (esModificado)
                        {
                            // UPDATE - Actualizar integrante existente
                            string query = "UPDATE Equipo_participa SET Apellido = ?, Nombre = ?, Telefono = ?, " +
                                          "Transporte = ?, Segundo = ?, Alergias = ?, Asistencia = ?, Comentario = ? " +
                                          "WHERE Id = ?";
                            OleDbCommand cmd = new OleDbCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Apellido", row["Apellido"]);
                            cmd.Parameters.AddWithValue("@Nombre", row["Nombre"]);
                            cmd.Parameters.AddWithValue("@Telefono", row["Telefono"]);
                            cmd.Parameters.AddWithValue("@Transporte", row["Transporte"]);
                            cmd.Parameters.AddWithValue("@Segundo", row["Segundo"]);
                            cmd.Parameters.AddWithValue("@Alergias", row["Alergias"]);
                            cmd.Parameters.AddWithValue("@Asistencia", row["Asistencia"]);
                            cmd.Parameters.AddWithValue("@Comentario", row["Comentario"]);
                            cmd.Parameters.AddWithValue("@Id", row["Id"]);

                            cmd.ExecuteNonQuery();
                            cambiosRealizados++;
                        }
                    }
                }

                MostrarMensajeExito($"✓ {cambiosRealizados} cambios guardados exitosamente en la base de datos.");

                // Limpiar cambios pendientes
                HayCambiosPendientes = false;
                Session["IntegrantesPendientes"] = null;

                // Recargar desde la BD
                CargarIntegrantes();
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al guardar los cambios en la base de datos: " + ex.Message);
            }
        }

        private void BorrarIntegranteTemporal(int id)
        {
            try
            {
                DataTable dt = IntegrantesPendientes;
                DataRow row = dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("Id") == id);

                if (row != null)
                {
                    // Si es un registro nuevo, simplemente eliminarlo de la lista temporal
                    if (Convert.ToBoolean(row["EsNuevo"]))
                    {
                        dt.Rows.Remove(row);
                        MostrarMensajeExito("Integrante eliminado de la lista.");
                    }
                    else
                    {
                        // Si es un registro existente en BD, eliminarlo ahora
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            string query = "DELETE FROM Equipo_participa WHERE Id = ?";
                            OleDbCommand cmd = new OleDbCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Id", id);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }

                        dt.Rows.Remove(row);
                        MostrarMensajeExito("Integrante eliminado exitosamente.");
                    }

                    IntegrantesPendientes = dt;
                    HayCambiosPendientes = dt.AsEnumerable().Any(r => Convert.ToBoolean(r["EsNuevo"]) || Convert.ToBoolean(r["EsModificado"]));
                    CargarIntegrantes();
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Error al eliminar el integrante: " + ex.Message);
            }
        }

        private void ActualizarBotonGuardarCambios()
        {
            // Mostrar el botón solo si hay cambios pendientes
            DataTable dt = IntegrantesPendientes;
            bool hayCambios = dt.AsEnumerable().Any(r => Convert.ToBoolean(r["EsNuevo"]) || Convert.ToBoolean(r["EsModificado"]));

            btnGuardarCambios.Visible = hayCambios;
        }

        private void LimpiarFormulario()
        {
            txtApellido.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            ddlTransporte.SelectedIndex = 0;
            txtSegundo.Text = string.Empty;
            txtAlergias.Text = string.Empty;
            chkAsistencia.Checked = false;
            txtComentario.Text = string.Empty;
            hdnIntegranteId.Value = "0";
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