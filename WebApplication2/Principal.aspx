<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Principal.aspx.cs" Inherits="WebApplication2.Principal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Torneo de Golf - Equipo</title>
    <link href="Content/styles.css" rel="stylesheet" type="text/css" />
    <style>
        .radio-list-responsive {
            width: 100%;
            margin-top: 5px;
        }

        /* Forzamos que las columnas de la tabla que genera el RadioButtonList ocupen exactamente la mitad (50%) */
        .radio-list-responsive td {
            width: 50%;
            text-align: center; /* Opcional: Centra el circulo y el texto dentro de su mitad */
        }

        /* Opcional: Agregamos un poco de espacio entre el circulito y el texto "Sí" / "No" */
        .radio-list-responsive label {
            margin-left: 5px;
            cursor: pointer;
        }

        /* Aumentar tamaño y centrar el nombre */
        .integrante-edit-grid .form-group.full {
            text-align: center;
        }
        .integrante-edit-grid .form-group.full label {
            font-size: 1.2em;
        }
        .integrante-edit-grid .form-group.full .readonly-field {
            display: block;
            font-size: 1.5em; /* Fuente más grande */
            font-weight: bold;
            margin-top: 8px;
        }

        /* Borde redondeado y verde oscuro para todos los form-group */
        .form-group {
            border: 2px solid darkgreen;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 15px;
        }

        /* Evitar que el botón de enviar tenga este borde */
        .comentarios-section .form-group:last-child {
            border: none;
            padding: 0;
        }
    </style>
</head>
<body>
    <img src="/images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

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
                                            CssClass="" Width="100%">
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
                                            CssClass="" Width="100%">
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

            <!-- Sección de Comentarios -->
            <div class="comentarios-section">
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
    </form>
</body>
</html>
