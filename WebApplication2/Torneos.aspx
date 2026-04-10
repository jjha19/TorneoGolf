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
    <!-- Mismo fondo que la pantalla principal -->
    <img src="images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">

            <!-- Título -->
            <div id="TitleText">
                <h1>Mis Torneos</h1>
                <div class="torneo-comentario">
                    <asp:Label ID="lblInstrucciones" runat="server" Text="Aquí tienes la lista de los torneos en los que participas."></asp:Label>
                </div>
            </div>

            <!-- Panel en caso de errores en la base de datos -->
            <asp:Panel ID="pnlMensajeError" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
            </asp:Panel>

            <h2>Listado de Torneos</h2>

            <!-- Lista de Torneos aprovechando la clase 'integrantes-lista' de styles.css para que parezcan tarjetas -->
            <div id="lista-container">
                <asp:Repeater ID="rptTorneos" runat="server">
                    <HeaderTemplate>
                        <ul class="integrantes-lista">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="integrante-item" style="padding: 20px; text-align: center; cursor: pointer; transition: transform 0.2s;">
                            <!-- Convertimos el ítem en un enlace que lleva a Participantes.aspx mandando el t_codigo -->
                            <a href='Participantes.aspx?torneo=<%# Eval("t_codigo") %>' style="text-decoration: none; color: inherit; display: block;">
                                <h3 style="margin: 0; color: #333;"><%# Eval("t_nombre") %></h3>
                            </a>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <!-- Panel a mostrar si el usuario no tiene torneos -->
                <asp:Panel ID="pnlNoData" runat="server" CssClass="no-data" Visible="false">
                    Actualmente no estás asignado a ningún torneo.
                </asp:Panel>
            </div>

        </div>
    </form>
</body>
</html>
