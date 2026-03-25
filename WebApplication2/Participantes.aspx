<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Participantes.aspx.cs" Inherits="WebApplication2.Participantes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Torneo de Golf - Participantes</title>
    <!-- Referencia única y centralizada a TODOS los estilos -->
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">
            
            <!-- Botón de Volver -->
            <div class="btn-volver-container">
                <a href="Torneos.aspx" class="btn-volver">&larr; Volver a Mis Torneos</a>
            </div>

            <!-- Título -->
            <div id="TitleText">
                <h1>Invitados: <asp:Label ID="lblNombreTorneo" runat="server"></asp:Label></h1>
                <div class="form-group" style="text-align: center; border: none; padding: 0;">
                    <asp:Button ID="btnDescargar" runat="server" Text="Descargar info" CssClass="btn-enviar-comentario" OnClick="btnDescargar_Click" />
                </div>
            </div>

            <!-- Mensajes Globales -->
            <asp:Panel ID="pnlMensajeExito" runat="server" CssClass="mensaje mensaje-exito" Visible="false">
                <asp:Label ID="lblMensajeExito" runat="server"></asp:Label>
            </asp:Panel>
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
                            <!-- Nombre del participante -->
                            <div class="participante-header">
                                <asp:Label ID="lblEditNombre" runat="server" Text='<%# string.Concat(Eval("p_nombre"), " ", Eval("p_apellido"), " - Equipo: ", Eval("e_codigo")) %>' CssClass="integrante-nombre-texto" style="font-weight: bold; font-size: 18px; color: #2E7D32;"></asp:Label>
                            </div>

                            <div class="integrante-edit-grid participante-grid-detalles">
                                
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

                                <div class="form-group">
                                    <strong>¿Jornada de práctica?:</strong>
                                    <asp:Label ID="lblPractica" runat="server" Text='<%# string.IsNullOrEmpty(Eval("p_practica")?.ToString()) ? "Sin respuesta" : Eval("p_practica") %>'></asp:Label>
                                </div>

                                <!-- Comentario a fila completa -->
                                <div class="form-group grid-full-width">
                                    <strong>Comentarios:</strong>
                                    <asp:Label ID="lblComentario" runat="server" CssClass="comentario-texto" 
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

            <!-- Este es el formulario para añadir un nuevo participante -->
            <div class="form-add-container">
                <h3>Añadir Nuevo Participante</h3>
                <div class="integrante-edit-grid participante-grid-add">
                    
                    <div class="form-group">
                        <label><strong>Nombre <span class="asterisco-req">*</span></strong></label>
                        <asp:TextBox ID="txtAddNombre" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvAddNombre" runat="server" ControlToValidate="txtAddNombre" ErrorMessage="El nombre es obligatorio" Display="Dynamic" ForeColor="Red" ValidationGroup="AddGroup"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <label><strong>Apellido <span class="asterisco-req">*</span></strong></label>
                        <asp:TextBox ID="txtAddApellido" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvAddApellido" runat="server" ControlToValidate="txtAddApellido" ErrorMessage="El apellido es obligatorio" Display="Dynamic" ForeColor="Red" ValidationGroup="AddGroup"></asp:RequiredFieldValidator>
                    </div>
                    
                    <div class="form-group">
                        <label><strong>Código Equipo <span class="asterisco-req">*</span></strong></label>
                        <asp:TextBox ID="txtAddCodigoEquipo" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvAddCodigoEquipo" runat="server" ControlToValidate="txtAddCodigoEquipo" ErrorMessage="El código de equipo es obligatorio" Display="Dynamic" ForeColor="Red" ValidationGroup="AddGroup"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <label><strong>Móvil</strong></label>
                        <asp:TextBox ID="txtAddMovil" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><strong>Alergias</strong></label>
                        <asp:TextBox ID="txtAddAlergia" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><strong>¿Asiste al torneo?</strong></label>
                        <div class="radio-wrapper-15 radio-list-responsive">
                            <asp:RadioButtonList ID="rblAddAsistencia" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                <asp:ListItem Text="No" Value="No"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label><strong>¿Tiene transporte?</strong></label>
                        <div class="radio-wrapper-15 radio-list-responsive">
                            <asp:RadioButtonList ID="rblAddTransporte" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                <asp:ListItem Text="No" Value="No"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label><strong>¿Asiste a práctica?</strong></label>
                        <div class="radio-wrapper-15 radio-list-responsive">
                            <asp:RadioButtonList ID="rblAddPractica" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                <asp:ListItem Text="No" Value="No"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div class="form-group grid-full-width">
                        <label><strong>Comentario</strong></label>
                        <asp:TextBox ID="txtAddComentario" runat="server" TextMode="MultiLine" Rows="3" CssClass="comentario-textbox sublimado add-comentario-box"></asp:TextBox>
                    </div>

                </div>

                <div class="form-group btn-guardar-container">
                    <asp:Button ID="btnGuardarParticipante" runat="server" Text="Guardar Participante" CssClass="btn-enviar-comentario" OnClick="btnGuardarParticipante_Click" ValidationGroup="AddGroup" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
