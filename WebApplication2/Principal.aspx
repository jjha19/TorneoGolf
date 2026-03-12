<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Principal.aspx.cs" Inherits="WebApplication2.Principal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Torneo de Golf - Equipo</title>
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="/images/FondoGolf.jpeg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">

            <!-- Título -->
            <div id="TitleText">
                <h1>Torneo de Golf - Equipo</h1>
            </div>

            <!-- Información del torneo y equipo -->
            <asp:Panel ID="pnlUserInfo" runat="server" CssClass="user-info" Visible="true">
                <div>
                    <strong>Nombre del torneo:</strong> <asp:Label ID="lblNombreTorneo" runat="server"></asp:Label>
                </div>
                <div>
                    <strong>Nombre del equipo:</strong> <asp:Label ID="lblNombreEquipo" runat="server"></asp:Label>
                    <asp:Label ID="lblCapitanBadge" runat="server" CssClass="capitan-badge" Visible="false">CAPITÁN</asp:Label>
                </div>
            </asp:Panel>

            <!-- Mensajes -->
            <asp:Panel ID="pnlMensajeExito" runat="server" CssClass="mensaje mensaje-exito" Visible="false">
                <asp:Label ID="lblMensajeExito" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlMensajeError" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
            </asp:Panel>

            <h2>Listado de Integrantes</h2>

            <!-- Lista de integrantes -->
            <div id="lista-container">
                <asp:Repeater ID="rptIntegrantes" runat="server" OnItemCommand="rptIntegrantes_ItemCommand" OnItemDataBound="rptIntegrantes_ItemDataBound">
                    <HeaderTemplate>
                        <ul class="integrantes-lista">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="integrante-item">
                            <!-- Vista de solo lectura -->
                            <asp:Panel ID="pnlView" runat="server" CssClass="integrante-view">
                                <div class="integrante-linea"><strong>Apellido:</strong> <%# Eval("Apellido") %></div>
                                <div class="integrante-linea"><strong>Nombre:</strong> <%# Eval("Nombre") %></div>
                                <div class="integrante-linea"><strong>Teléfono:</strong> <%# Eval("Telefono") %></div>
                                <div class="integrante-linea"><strong>Transporte:</strong> <%# Eval("Transporte") %></div>
                                <div class="integrante-linea"><strong>Segundo:</strong> <%# Eval("Segundo") %></div>
                                <div class="integrante-linea"><strong>Alergias:</strong> <%# Eval("Alergias") %></div>
                                <div class="integrante-linea"><strong>Asistencia:</strong> <%# (bool)Eval("Asistencia") ? "Sí" : "No" %></div>
                                <div class="integrante-linea"><strong>Comentario:</strong> <%# Eval("Comentario") %></div>

                                <asp:Panel ID="pnlAcciones" runat="server" CssClass="integrante-acciones" Visible="false">
                                    <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn-editar" 
                                        CommandName="Editar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" />
                                    <asp:Button ID="btnBorrar" runat="server" Text="Borrar" CssClass="btn-borrar" 
                                        CommandName="Borrar" CommandArgument='<%# Eval("Id") %>' 
                                        OnClientClick="return confirm('¿Está seguro de eliminar este integrante?');" CausesValidation="false" />
                                </asp:Panel>
                            </asp:Panel>

                            <!-- Vista de edición inline -->
                            <asp:Panel ID="pnlEdit" runat="server" CssClass="integrante-edit" Visible="false">
                                <div class="integrante-edit-grid">
                                    <!-- Apellido y Nombre NO editables -->
                                    <div class="form-group">
                                        <label><strong>Apellido:</strong></label>
                                        <asp:Label ID="lblEditApellido" runat="server" Text='<%# Eval("Apellido") %>' CssClass="readonly-field"></asp:Label>
                                    </div>

                                    <div class="form-group">
                                        <label><strong>Nombre:</strong></label>
                                        <asp:Label ID="lblEditNombre" runat="server" Text='<%# Eval("Nombre") %>' CssClass="readonly-field"></asp:Label>
                                    </div>

                                    <!-- Campos editables -->
                                    <div class="form-group">
                                        <label>Teléfono:</label>
                                        <asp:TextBox ID="txtEditTelefono" runat="server" Text='<%# Eval("Telefono") %>'></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label>Transporte:</label>
                                        <asp:DropDownList ID="ddlEditTransporte" runat="server">
                                            <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <div class="form-group">
                                        <label>Segundo:</label>
                                        <asp:TextBox ID="txtEditSegundo" runat="server" Text='<%# Eval("Segundo") %>'></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label>Alergias:</label>
                                        <asp:TextBox ID="txtEditAlergias" runat="server" Text='<%# Eval("Alergias") %>'></asp:TextBox>
                                    </div>

                                    <div class="form-group">
                                        <label>
                                            <asp:CheckBox ID="chkEditAsistencia" runat="server" Checked='<%# Eval("Asistencia") %>' />
                                            Confirmar asistencia
                                        </label>
                                    </div>

                                    <div class="form-group full">
                                        <label>Comentario:</label>
                                        <asp:TextBox ID="txtEditComentario" runat="server" TextMode="MultiLine" 
                                            Rows="3" Text='<%# Eval("Comentario") %>'></asp:TextBox>
                                    </div>

                                    <!-- Botones de edición -->
                                    <div class="form-group full">
                                        <div class="integrante-acciones">
                                            <asp:Button ID="btnGuardarEdit" runat="server" Text="Guardar" CssClass="btn-editar" 
                                                CommandName="GuardarEdit" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" />
                                            <asp:Button ID="btnCancelarEdit" runat="server" Text="Cancelar" CssClass="btn-cancelar" 
                                                CommandName="CancelarEdit" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" />
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoData" runat="server" CssClass="no-data" Visible="false">
                    No hay integrantes registrados en el equipo.
                </asp:Panel>
            </div>

            <!-- Botones de acción (solo para capitanes) -->
            <div class="buttons-container">
                <asp:Button ID="btnMostrarFormulario" runat="server" Text="+ Agregar Integrante al Equipo" 
                    CssClass="btn-mostrar-form" OnClick="btnMostrarFormulario_Click" Visible="false" />
                <asp:Button ID="btnGuardarCambios" runat="server" Text="💾 Guardar cambios en BD" 
                    CssClass="btn-guardar-cambios" OnClick="btnGuardarCambios_Click" Visible="false" />
            </div>

            <!-- Formulario para agregar integrantes -->
            <asp:Panel ID="pnlFormAgregar" runat="server" CssClass="form-agregar" Visible="false">
                <h2><asp:Label ID="lblFormTitulo" runat="server" Text="Agregar Integrante"></asp:Label></h2>

                <div class="form-row">
                    <div class="form-group">
                        <label>Código de Equipo:</label>
                        <asp:TextBox ID="txtCodigoEquipo" runat="server" ReadOnly="true" Enabled="false"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Teléfono:</label>
                        <asp:TextBox ID="txtTelefono" runat="server" TextMode="Phone"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvTelefono" runat="server" 
                            ControlToValidate="txtTelefono" ErrorMessage="El teléfono es requerido" 
                            CssClass="error" Display="Dynamic" ValidationGroup="FormIntegrante">*</asp:RequiredFieldValidator>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label>Apellido:</label>
                        <asp:TextBox ID="txtApellido" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvApellido" runat="server" 
                            ControlToValidate="txtApellido" ErrorMessage="El apellido es requerido" 
                            CssClass="error" Display="Dynamic" ValidationGroup="FormIntegrante">*</asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <label>Nombre:</label>
                        <asp:TextBox ID="txtNombre" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNombre" runat="server" 
                            ControlToValidate="txtNombre" ErrorMessage="El nombre es requerido" 
                            CssClass="error" Display="Dynamic" ValidationGroup="FormIntegrante">*</asp:RequiredFieldValidator>
                    </div>
                </div>

                <div class="form-group">
                    <label>Transporte:</label>
                    <asp:DropDownList ID="ddlTransporte" runat="server">
                        <asp:ListItem Text="Sí" Value="Si" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="No" Value="No"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label>Segundo (Plato de comida):</label>
                        <asp:TextBox ID="txtSegundo" runat="server" placeholder="Ej: Pollo asado"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Alergias:</label>
                        <asp:TextBox ID="txtAlergias" runat="server" placeholder="Ej: Mariscos"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>
                            <asp:CheckBox ID="chkAsistencia" runat="server" />
                            Confirmar asistencia
                        </label>
                    </div>
                </div>

                <div class="form-group">
                    <label>Comentario:</label>
                    <asp:TextBox ID="txtComentario" runat="server" TextMode="MultiLine" 
                        Rows="3" placeholder="Observaciones adicionales..."></asp:TextBox>
                </div>

                <div class="form-buttons">
                    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                        CssClass="btn-cancelar" OnClick="btnCancelar_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardar" runat="server" Text="Agregar a tabla" 
                        CssClass="btn-agregar" OnClick="btnGuardar_Click" ValidationGroup="FormIntegrante" />
                </div>

                <asp:HiddenField ID="hdnIntegranteId" runat="server" Value="0" />
            </asp:Panel>

        </div>
    </form>
</body>
</html>
