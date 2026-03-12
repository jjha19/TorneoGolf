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
                <h1>Torneo: <asp:Label ID="lblTituloTorneo" runat="server"></asp:Label> - Equipo: <asp:Label ID="lblTituloEquipo" runat="server"></asp:Label></h1>
                <div class="torneo-comentario">
                    <asp:Label ID="lblComentarioTorneo" runat="server"></asp:Label>
                </div>
            </div>

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
                <asp:Repeater ID="rptIntegrantes" runat="server" OnItemCommand="rptIntegrantes_ItemCommand">
                    <HeaderTemplate>
                        <ul class="integrantes-lista">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="integrante-item">
                            <!-- Vista de solo lectura -->
                            <asp:Panel ID="pnlView" runat="server" CssClass="integrante-view">
                                <div class="integrante-linea"><strong>Nombre:</strong> <%# Eval("p_nombre") %></div>
                                <div class="integrante-linea">
                                    <strong>Asiste:</strong> 
                                    <span><%# Convert.ToBoolean(Eval("p_asistencia")) ? "Sí" : "No" %></span>
                                </div>
                                <div class="integrante-linea">
                                    <strong>Usará autobús:</strong> 
                                    <span><%# Convert.ToBoolean(Eval("p_transporte")) ? "Sí" : "No" %></span>
                                </div>
                                <div class="integrante-linea">
                                    <strong>Alergias:</strong> 
                                    <span><%# Eval("p_alergia") %></span>
                                </div>

                                <asp:Panel ID="pnlAcciones" runat="server" CssClass="integrante-acciones" Visible="true">
                                    <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn-editar" 
                                        CommandName="Editar" CommandArgument='<%# Eval("p_contador") %>' CausesValidation="false" />
                                </asp:Panel>
                            </asp:Panel>

                            <!-- Vista de edición inline -->
                            <asp:Panel ID="pnlEdit" runat="server" CssClass="integrante-edit" Visible="false">
                                <div class="integrante-edit-grid">
                                    <!-- Nombre NO editable -->
                                    <div class="form-group full">
                                        <label><strong>Nombre:</strong></label>
                                        <asp:Label ID="lblEditNombre" runat="server" Text='<%# Eval("p_nombre") %>' CssClass="readonly-field"></asp:Label>
                                    </div>

                                    <!-- Campos editables (checkboxes) -->
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkEditAsistencia" runat="server" Checked='<%# Convert.ToBoolean(Eval("p_asistencia")) %>' />
                                        <label style="display:inline; margin-left:5px;">Asiste</label>
                                    </div>

                                    <div class="form-group">
                                        <asp:CheckBox ID="chkEditTransporte" runat="server" Checked='<%# Convert.ToBoolean(Eval("p_transporte")) %>' />
                                        <label style="display:inline; margin-left:5px;">Usará autobús</label>
                                    </div>

                                    <!-- Botones de edición -->
                                    <div class="form-group full">
                                        <div class="integrante-acciones">
                                            <asp:Button ID="btnGuardarEdit" runat="server" Text="Guardar" CssClass="btn-editar" 
                                                CommandName="GuardarEdit" CommandArgument='<%# Eval("p_contador") %>' CausesValidation="false" />
                                            <asp:Button ID="btnCancelarEdit" runat="server" Text="Cancelar" CssClass="btn-cancelar" 
                                                CommandName="CancelarEdit" CommandArgument='<%# Eval("p_contador") %>' CausesValidation="false" />
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

            <!-- Sección de Comentarios -->
            <div class="comentarios-section">
                <h2>Comentarios o Sugerencias</h2>
                <div class="form-group">
                    <asp:TextBox ID="txtComentario" runat="server" TextMode="MultiLine" 
                        Rows="5" placeholder="Escribe aquí tus comentarios o sugerencias..." 
                        CssClass="comentario-textbox"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Button ID="btnEnviarComentario" runat="server" Text="Enviar" 
                        CssClass="btn-enviar-comentario" OnClick="btnEnviarComentario_Click" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
