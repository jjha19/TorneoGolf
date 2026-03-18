<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Principal.aspx.cs" Inherits="WebApplication2.Principal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Torneo de Golf - Equipo</title>
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

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
                            <asp:HiddenField ID="hdnContador" runat="server" Value='<%# Eval("p_contador") %>' />
                            
                            <div class="integrante-edit-grid">
                                <!-- Nombre NO editable -->
                                <div class="form-group full">
                                    <label><strong>Nombre:</strong></label>
                                    <asp:Label ID="lblEditNombre" runat="server" Text='<%# Eval("p_nombre") %>' CssClass="readonly-field"></asp:Label>
                                </div>

                                <!-- Asistencia RadioButtonList -->
                                <div class="form-group">
                                    <label><strong>Asiste:</strong></label>
                                    <div class="radio-wrapper-15 radio-list-responsive">
                                        <asp:RadioButtonList ID="rblAsistencia" runat="server" RepeatDirection="Horizontal" 
                                            SelectedValue='<%# Convert.ToString(Eval("p_asistencia")) == "Si" || Convert.ToString(Eval("p_asistencia")) == "Sí" ? "Si" : (Convert.ToString(Eval("p_asistencia")) == "No" ? "No" : null) %>' 
                                            CssClass="rbl-asistencia" Width="100%">
                                        <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <asp:RequiredFieldValidator ID="rfvAsistencia" runat="server" 
                                        ControlToValidate="rblAsistencia" ErrorMessage="Requerido" 
                                        Display="Dynamic" CssClass="mensaje-error">Este campo es Obligatorio</asp:RequiredFieldValidator>
                                </div>

                                <!-- Transporte RadioButtonList -->
                                <div class="form-group">
                                    <label><strong>Usará autobús:</strong></label>
                                    <div class="radio-wrapper-15 radio-list-responsive">
                                        <asp:RadioButtonList ID="rblTransporte" runat="server" RepeatDirection="Horizontal" 
                                            SelectedValue='<%# Convert.ToString(Eval("p_transporte")) == "Si" || Convert.ToString(Eval("p_transporte")) == "Sí" ? "Si" : (Convert.ToString(Eval("p_transporte")) == "No" ? "No" : null) %>' 
                                            CssClass="rbl-transporte" Width="100%">
                                        <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <asp:RequiredFieldValidator ID="rfvTransporte" runat="server" 
                                        ControlToValidate="rblTransporte" ErrorMessage="Requerido" 
                                        Display="Dynamic" CssClass="mensaje-error">Este campo es Obligatorio</asp:RequiredFieldValidator>
                                </div>

                                <!-- Alergias -->
                                <div class="form-group">
                                    <label><strong>Alergias / Intolerancias:</strong></label>
                                    <asp:TextBox ID="txtAlergia" runat="server" Text='<%# Eval("p_alergia") %>' CssClass="textbox-alergia"></asp:TextBox>
                                </div>
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

            <!-- Pregunta práctica previa a comentarios -->
            <div class="comentarios-section">
                <div class="form-group">
                    <label><strong>¿Asistirá a la jornada de práctica?</strong></label>
                    <div class="radio-wrapper-15 radio-list-responsive">
                        <asp:RadioButtonList ID="rblPractica" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>

                <!-- Sección de Comentarios -->
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
        <script type="text/javascript">
            (function () {
                function onReady() {
                    var asistenciaInputs = document.querySelectorAll(".rbl-asistencia input[type='radio']");
                    asistenciaInputs.forEach(function (input) {
                        input.addEventListener("change", function () {
                            if (input.checked && input.value === "No") {
                                var item = input.closest(".integrante-item");
                                if (!item) {
                                    return;
                                }
                                var transporteInputs = item.querySelectorAll(".rbl-transporte input[type='radio']");
                                transporteInputs.forEach(function (transporte) {
                                    if (transporte.value === "No") {
                                        transporte.checked = true;
                                    }
                                });
                            }
                        });
                    });
                }

                if (document.readyState === "loading") {
                    document.addEventListener("DOMContentLoaded", onReady);
                } else {
                    onReady();
                }
            })();
        </script>
    </form>
</body>
</html>
