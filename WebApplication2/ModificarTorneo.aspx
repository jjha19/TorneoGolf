<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModificarTorneo.aspx.cs" Inherits="WebApplication2.ModificarTorneo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Modificar Torneo</title>
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
                <h1>Modificar Torneo</h1>
            </div>

            <asp:ValidationSummary ID="vsErrores" runat="server" CssClass="mensaje-validacion"
                DisplayMode="SingleParagraph" ShowSummary="true" HeaderText="Revisa los campos obligatorios" />

            <asp:Panel ID="pnlMensajeExito" runat="server" CssClass="mensaje mensaje-exito" Visible="false">
                <asp:Label ID="lblMensajeExito" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlMensajeError" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
            </asp:Panel>

            <div class="comentarios-section">
                <h2>Datos del torneo</h2>

                <div class="integrante-edit-grid">
                    <div class="form-group">
                        <label><strong>Código del torneo</strong></label>
                        <asp:TextBox ID="txtCodigo" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCodigo" runat="server" ControlToValidate="txtCodigo"
                            ErrorMessage="El código es obligatorio" Display="Dynamic" CssClass="mensaje-error">*</asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <label><strong>Nombre del torneo</strong></label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre"
                            ErrorMessage="El nombre es obligatorio" Display="Dynamic" CssClass="mensaje-error">*</asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <label><strong>Contacto</strong></label>
                        <asp:TextBox ID="txtContacto" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><strong>Móvil</strong></label>
                        <asp:TextBox ID="txtMovil" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><strong>Fecha</strong></label>
                        <asp:TextBox ID="txtFecha" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><strong>Usuario</strong></label>
                        <asp:TextBox ID="txtUsuario" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><strong>Contraseña</strong></label>
                        <asp:TextBox ID="txtContrasena" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group grid-full-width">
                        <label><strong>Comentario</strong></label>
                        <asp:TextBox ID="txtComentario" runat="server" TextMode="MultiLine" Rows="4" CssClass="comentario-textbox"></asp:TextBox>
                    </div>

                    <div class="form-group grid-full-width">
                        <label><strong>Texto Whatsapp</strong></label>
                        <asp:TextBox ID="txtTextoWs" runat="server" TextMode="MultiLine" Rows="5" CssClass="comentario-textbox"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group" style="text-align: center !important;">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar cambios" CssClass="btn-enviar-comentario" OnClick="btnGuardar_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
