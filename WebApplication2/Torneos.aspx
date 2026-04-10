<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Torneos.aspx.cs" Inherits="WebApplication2.Torneos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Torneo de Golf - Mis Torneos</title>
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">

            <div id="TitleText">
                <h1>Mis Torneos</h1>
                <div class="torneo-comentario">
                    <asp:Label ID="lblInstrucciones" runat="server" Text="Aquí tienes la lista de los torneos en los que participas."></asp:Label>
                </div>
            </div>

            <asp:Panel ID="pnlMensajeError" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
            </asp:Panel>

            <h2>Listado de Torneos</h2>

            <div id="lista-container">
                <asp:Repeater ID="rptTorneos" runat="server">
                    <HeaderTemplate>
                        <ul class="integrantes-lista">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="integrante-item torneo-item">
                            <div class="torneo-item-contenido">
                                <h3 class="torneo-item-titulo"><%# Eval("t_nombre") %></h3>

                                <div class="torneo-acciones">
                                    <a href='Participantes.aspx?torneo=<%# Eval("t_codigo") %>' class="btn-enviar-comentario torneo-btn-link">
                                        Ver participantes
                                    </a>
                                    <a href='ModificarTorneo.aspx?torneoId=<%# Eval("t_contador") %>' class="btn-enviar-comentario torneo-btn-link btn-modificar-torneo">
                                        Modificar torneo
                                    </a>
                                </div>
                            </div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoData" runat="server" CssClass="no-data" Visible="false">
                    Actualmente no estás asignado a ningún torneo.
                </asp:Panel>
            </div>

        </div>
    </form>
</body>
</html>
