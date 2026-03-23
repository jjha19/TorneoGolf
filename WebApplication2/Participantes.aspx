<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Participantes.aspx.cs" Inherits="WebApplication2.Participantes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
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
                <div class="form-group" style="text-align: center; border: none; padding: 0;">
                    <asp:Button ID="btnDescargar" runat="server" Text="Descargar info" CssClass="btn-enviar-comentario" OnClick="btnDescargar_Click" />
                </div>
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
                            <!-- Nombre principal del participante -->
                            <div class="participante-nombre">
                                <asp:Label ID="lblEditNombre" runat="server" Text='<%# string.Concat(Eval("p_nombre"), " ", Eval("p_apellido")) %>' CssClass="integrante-nombre-texto"></asp:Label>
                            </div>

                            <div class="participantes-detalles">
                                
                                <div class="form-group">
                                    <strong>Móvil:</strong>
                                    <asp:Label ID="lblMovil" runat="server" Text='<%# string.IsNullOrEmpty(Eval("p_movi")?.ToString()) ? "No especificado" : Eval("p_movi") %>'></asp:Label>
                                </div>

                                <div class="form-group">
                                    <strong>Alergias:</strong>
                                    <asp:Label ID="lblAlergias" runat="server" Text='<%# string.IsNullOrEmpty(Eval("p_alergia")?.ToString()) ? "Ninguna" : Eval("p_alergia") %>'></asp:Label>
                                </div>

                                <div class="form-group">
                                    <strong>¿Asiste al torneo?:</strong>
                                    <asp:Label ID="lblAsiste" runat="server" Text='<%# string.IsNullOrEmpty(Eval("p_asistencia")?.ToString()) ? "Sin respuesta" : Eval("p_asistencia") %>'></asp:Label>
                                </div>

                                <div class="form-group">
                                    <strong>Usará autobús:</strong>
                                    <asp:Label ID="lblTransporte" runat="server" Text='<%# string.IsNullOrEmpty(Eval("p_transporte")?.ToString()) ? "Sin respuesta" : Eval("p_transporte") %>'></asp:Label>
                                </div>

                                <!-- Añadido nuevo campo: Jornada de práctica -->
                                <div class="form-group">
                                    <strong>¿Jornada de práctica?:</strong>
                                    <asp:Label ID="lblPractica" runat="server" Text='<%# string.IsNullOrEmpty(Eval("p_practica")?.ToString()) ? "Sin respuesta" : Eval("p_practica") %>'></asp:Label>
                                </div>

                                <!-- Comentario a fila completa -->
                                <div class="form-group">
                                    <strong>Comentarios:</strong>
                                    <asp:Label ID="lblComentario" runat="server" style="color: #666; font-style: italic;" 
                                        Text='<%# string.IsNullOrEmpty(Eval("p_comentario")?.ToString()) ? "Sin comentarios..." : Eval("p_comentario") %>'></asp:Label>
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
