<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AñadirEquipo.aspx.cs" Inherits="WebApplication2.AñadirEquipo" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Añadir Equipo</title>
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
                <h1>Añadir equipo al torneo</h1>
                <div class="torneo-comentario">
                    <asp:Label ID="lblTorneoActual" runat="server"></asp:Label>
                </div>
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
                <h2>Datos del equipo</h2>

                <div class="integrante-edit-grid">
                    <div class="form-group">
                        <label><strong>Código del equipo</strong></label>
                        <asp:TextBox ID="txtCodigoEquipo" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCodigoEquipo" runat="server" ControlToValidate="txtCodigoEquipo"
                            ErrorMessage="El código del equipo es obligatorio" Display="Dynamic" CssClass="mensaje-error">*</asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <label><strong>Nombre del equipo</strong></label>
                        <asp:TextBox ID="txtNombreEquipo" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNombreEquipo" runat="server" ControlToValidate="txtNombreEquipo"
                            ErrorMessage="El nombre del equipo es obligatorio" Display="Dynamic" CssClass="mensaje-error">*</asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <label><strong>Móvil</strong></label>
                        <asp:TextBox ID="txtMovilEquipo" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><strong>Tlf de Contacto</strong></label>
                        <asp:TextBox ID="txtContactoEquipo" runat="server" CssClass="textbox-alergia"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group" style="text-align: center !important;">
                    <asp:Button ID="btnGuardarEquipo" runat="server" Text="Guardar equipo" CssClass="btn-enviar-comentario" OnClick="btnGuardarEquipo_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
