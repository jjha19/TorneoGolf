<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Agradecimiento.aspx.cs" Inherits="WebApplication2.Agradecimiento" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Torneo de Golf - Equipo</title>
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
</head>
<body class="agradecimiento-page">
    <img src="images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">

            <div id="TitleText">
                <h1>Muchas gracias por su colaboracion</h1>
            </div>
            <div class="comentarios-section">
                <img src="images/tick.png" alt="Tick de Agradecimiento" id="tick-image"/>
                <h2>Sus datos se han guardado Correctamente</h2>
                <div class="form-group" style="text-align: center !important;">
                    <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar" CssClass="btn-enviar-comentario" PostBackUrl="https://aesgolf.com/" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
