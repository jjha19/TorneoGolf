<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaEquipos.aspx.cs" Inherits="WebApplication2.ListaEquipos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Torneo de Golf - Equipos</title>
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">

            <div id="TitleText">
                <div class="btn-volver-container">
                    <a href="Torneos.aspx" class="btn-volver">&larr; <span class="btn-volver-texto">Volver a Mis Torneos</span></a>
                </div>
                <h1>Equipos del torneo</h1>
                <div class="torneo-comentario">
                    <asp:Label ID="lblTorneo" runat="server"></asp:Label>
                </div>
            </div>

            <asp:Panel ID="pnlMensajeError" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
            </asp:Panel>

            <div id="lista-container">
                <asp:Repeater ID="rptEquipos" runat="server">
                    <HeaderTemplate>
                        <ul class="integrantes-lista">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="integrante-item">
                            <div class="integrante-edit-grid">
                                <div class="form-group">
                                    <strong>Código equipo:</strong><br />
                                    <%# Eval("e_codigo") %>
                                </div>
                                <div class="form-group">
                                    <strong>Nombre equipo:</strong><br />
                                    <%# Eval("e_nombre") %>
                                </div>
                                <div class="form-group">
                                    <strong>Móvil:</strong><br />
                                    <%# Eval("e_movil") %>
                                </div>
                                <div class="form-group">
                                    <strong>Tlf contacto:</strong><br />
                                    <%# Eval("e_contacto") %>
                                </div>
                            </div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoData" runat="server" CssClass="no-data" Visible="false">
                    No hay equipos registrados para este torneo.
                </asp:Panel>
            </div>

        </div>
    </form>
</body>
</html>
