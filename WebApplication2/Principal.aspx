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
                <h1><asp:Label ID="lblTituloTorneo" runat="server"></asp:Label> - Equipo: <asp:Label ID="lblTituloEquipo" runat="server"></asp:Label></h1>
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
                            <div class="integrante-contenido">
                                <!-- Nombre grande y destacado (solo lectura) -->
                                <div class="integrante-nombre-grande" style="font-size: 26px !important; font-weight: 600 !important; text-align: center !important; color: black !important;">
                                    <%# Eval("p_nombre") %>
                                </div>

                                <!-- Checkboxes editables directamente -->
                                <div class="integrante-campos">
                                    <div class="campo-checkbox">
                                        <asp:CheckBox ID="chkAsistencia" runat="server" Checked='<%# Convert.ToBoolean(Eval("p_asistencia")) %>' />
                                        <label>Asistirá</label>
                                    </div>

                                    <div class="campo-checkbox">
                                        <asp:CheckBox ID="chkTransporte" runat="server" Checked='<%# Convert.ToBoolean(Eval("p_transporte")) %>' />
                                        <label>Usará autobús</label>
                                    </div>

                                    <div class="campo-alergia">
                                        <strong>Alergias:</strong>
                                        <asp:TextBox ID="txtAlergia" runat="server" Text='<%# Eval("p_alergia") %>' 
                                            placeholder="Ej. Mariscos"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- HiddenField para guardar el ID -->
                                <asp:HiddenField ID="hdnContador" runat="server" Value='<%# Eval("p_contador") %>' />
                            </div>
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
                <div class="form-group" style="text-align: center !important;">
                    <asp:Button ID="btnEnviarComentario" runat="server" Text="Guardar y Enviar" 
                        CssClass="btn-enviar-comentario" OnClick="btnEnviarComentario_Click" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
