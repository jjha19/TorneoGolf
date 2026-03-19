<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Participantes.aspx.cs" Inherits="WebApplication2.Participantes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Torneo de Golf - Participantes</title>
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="/images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">
            
            <!-- Botón de Volver -->
            <div style="margin-bottom: 20px;">
                <a href="Torneos.aspx" style="color: #4CAF50; text-decoration: none; font-weight: bold; font-size: 16px;">&larr; Volver a Mis Torneos</a>
            </div>

            <!-- Título -->
            <div id="TitleText">
                <h1>Participantes: <asp:Label ID="lblNombreTorneo" runat="server"></asp:Label></h1>
            </div>

            <!-- Mensajes -->
            <asp:Panel ID="pnlMensajeError" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Lista de participantes -->
            <div id="lista-container">
                <asp:Repeater ID="rptParticipantes" runat="server">
                    <HeaderTemplate>
                        <ul class="integrantes-lista">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="integrante-item">
                            <div class="integrante-edit-grid" style="grid-template-columns: 1fr;">
                                <div class="form-group full" style="font-size: 18px;">
                                    <strong><%# Eval("p_nombre") %> <%# Eval("p_apellido") %></strong>
                                </div>
                                <div class="form-group full" style="margin-top: 10px;">
                                    <strong>Móvil:</strong> <%# Eval("p_movi") %>
                                </div>
                            </div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoData" runat="server" CssClass="no-data" Visible="false">
                    No hay participantes registrados en este torneo.
                </asp:Panel>
            </div>

        </div>
    </form>
</body>
</html>
