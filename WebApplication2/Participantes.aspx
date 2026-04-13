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
    <img src="images/golfMadridA.jpg" alt="Fondo de Golf" id="bg_image" />

    <form id="form1" runat="server">
        <div class="container">
            
            <div id="TitleText">
                <div class="btn-volver-container">
                    <a href="Torneos.aspx" class="btn-volver">
                        &larr; <span class="btn-volver-texto">Volver a Mis Torneos</span>
                    </a>
                </div>
                <h1>Invitados: <asp:Label ID="lblNombreTorneo" runat="server"></asp:Label></h1>
                <div class="form-group" style="text-align: center; border: none; padding: 0;">
                    <asp:Button ID="btnDescargar" runat="server" Text="Descargar info" CssClass="btn-enviar-comentario" OnClick="btnDescargar_Click" />
                </div>
                <div class="search-bar-container">
                    <div class="search-bar">
                        <input type="text" class="search-input" placeholder="Buscar" id="searchInput" />
                        <button type="button" class="search-button" id="searchButton">
                            <img src="images/lupa.png" alt="Buscar" class="search-icon" />
                        </button>
                    </div>
                </div>
            </div>

            <div class="estadisticas" id="estadisticasContainer">
                <div class="estadisticas-total">
                    Invitados en total: <asp:Label ID="lblInvitadosTotal" runat="server"></asp:Label>
                </div>
                <div class="estadisticas-col">
                    <div class="estadisticas-title">Asistencia</div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">Asisten:</span>
                        <asp:Label ID="lblAsistenSi" runat="server"></asp:Label>
                    </div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">No asisten:</span>
                        <asp:Label ID="lblAsistenNo" runat="server"></asp:Label>
                    </div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">Sin confirmar:</span>
                        <asp:Label ID="lblAsistenPendiente" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="estadisticas-col">
                    <div class="estadisticas-title">Jornada de Práctica</div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">Asisten:</span>
                        <asp:Label ID="lblPracticaSi" runat="server"></asp:Label>
                    </div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">No asisten:</span>
                        <asp:Label ID="lblPracticaNo" runat="server"></asp:Label>
                    </div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">Sin confirmar:</span>
                        <asp:Label ID="lblPracticaPendiente" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="estadisticas-col">
                    <div class="estadisticas-title">Autobús</div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">Usa autobús:</span>
                        <asp:Label ID="lblTransporteSi" runat="server"></asp:Label>
                    </div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">No usa:</span>
                        <asp:Label ID="lblTransporteNo" runat="server"></asp:Label>
                    </div>
                    <div class="estadisticas-item">
                        <span class="estadisticas-label">Sin confirmar:</span>
                        <asp:Label ID="lblTransportePendiente" runat="server"></asp:Label>
                    </div>
                </div>
            </div>

            <!-- Resumen compacto tipo Excel (PASO 3: debajo de estadísticas) -->
            <div class="resumen-compacto">
                <h3>Resumen rápido de equipos e integrantes</h3>
                <asp:GridView ID="gvResumenCompacto" runat="server" AutoGenerateColumns="false"
                    CssClass="tabla-compacta" GridLines="Both" ShowHeaderWhenEmpty="true" EmptyDataText="Sin datos"
                    OnRowDataBound="gvResumenCompacto_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="e_codigo" HeaderText="Equipo" />
                        <asp:BoundField DataField="equipo_nombre" HeaderText="Nombre equipo" />
                        <asp:BoundField DataField="p_nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="p_apellido" HeaderText="Apellido" />
                        <asp:BoundField DataField="p_asistencia" HeaderText="Asiste" />
                        <asp:BoundField DataField="p_transporte" HeaderText="Bus" />
                        <asp:BoundField DataField="p_practica" HeaderText="Práctica" />
                        <asp:BoundField DataField="fecha_ws" HeaderText="Fecha mensaje WhatsApp" />
                        <asp:BoundField DataField="p_comentario" HeaderText="Comentarios" />
                    </Columns>
                </asp:GridView>
            </div>

            <asp:Panel ID="pnlMensajeExito" runat="server" CssClass="mensaje mensaje-exito" Visible="false">
                <asp:Label ID="lblMensajeExito" runat="server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlMensajeError" runat="server" CssClass="mensaje mensaje-error" Visible="false">
                <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
            </asp:Panel>

            <div id="lista-container">
                <asp:Repeater ID="rptParticipantes" runat="server" OnItemCommand="rptParticipantes_ItemCommand" OnItemDataBound="rptParticipantes_ItemDataBound">
                    <HeaderTemplate>
                        <ul class="integrantes-lista participantes-lista">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phEquipoHeader" runat="server" />
                        <li id="itemContainer" runat="server" class="integrante-item">
                            <asp:HiddenField ID="hdnContador" runat="server" Value='<%# Eval("p_contador") %>' />

                            <div class="integrante-edit-grid participante-grid-detalles">
                                <div class="form-group">
                                    <strong>Nombre:</strong>
                                    <asp:TextBox ID="txtEditNombre" runat="server" CssClass="textbox-alergia"
                                        Text='<%# Eval("p_nombre") %>'></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <strong>Apellido:</strong>
                                    <asp:TextBox ID="txtEditApellido" runat="server" CssClass="textbox-alergia"
                                        Text='<%# Eval("p_apellido") %>'></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <strong>Equipo:</strong>
                                    <asp:TextBox ID="txtEditEquipo" runat="server" CssClass="textbox-alergia"
                                        Text='<%# Eval("e_codigo") %>'></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <strong>Móvil:</strong>
                                    <asp:TextBox ID="txtEditMovil" runat="server" CssClass="textbox-alergia"
                                        Text='<%# Eval("p_movi") %>'></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <strong>Alergias:</strong>
                                    <asp:TextBox ID="txtEditAlergias" runat="server" CssClass="textbox-alergia"
                                        Text='<%# Eval("p_alergia") %>'></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <strong>¿Asiste al torneo?:</strong>
                                    <div class="radio-wrapper-15 radio-list-responsive">
                                        <asp:RadioButtonList ID="rblEditAsistencia" runat="server" RepeatDirection="Horizontal"
                                            SelectedValue='<%# Convert.ToString(Eval("p_asistencia")) == "Si" || Convert.ToString(Eval("p_asistencia")) == "Sí" ? "Si" : (Convert.ToString(Eval("p_asistencia")) == "No" ? "No" : null) %>'>
                                            <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <strong>Usará autobús:</strong>
                                    <div class="radio-wrapper-15 radio-list-responsive">
                                        <asp:RadioButtonList ID="rblEditTransporte" runat="server" RepeatDirection="Horizontal"
                                            SelectedValue='<%# Convert.ToString(Eval("p_transporte")) == "Si" || Convert.ToString(Eval("p_transporte")) == "Sí" ? "Si" : (Convert.ToString(Eval("p_transporte")) == "No" ? "No" : null) %>'>
                                            <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <strong>¿Jornada de práctica?:</strong>
                                    <div class="radio-wrapper-15 radio-list-responsive">
                                        <asp:RadioButtonList ID="rblEditPractica" runat="server" RepeatDirection="Horizontal"
                                            SelectedValue='<%# Convert.ToString(Eval("p_practica")) == "Si" || Convert.ToString(Eval("p_practica")) == "Sí" ? "Si" : (Convert.ToString(Eval("p_practica")) == "No" ? "No" : null) %>'>
                                            <asp:ListItem Text="Sí" Value="Si"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <strong>Fecha Mensaje Whatsapp:</strong>
                                    <asp:TextBox ID="txtEditFechaWs" runat="server" CssClass="textbox-alergia"
                                        Text='<%# string.IsNullOrEmpty(Eval("fecha_ws")?.ToString()) ? "Todavía no se ha enviado" : Convert.ToDateTime(Eval("fecha_ws")).ToString("dd/MM/yyyy HH:mm:ss") %>'></asp:TextBox>
                                </div>

                                <div class="form-group grid-full-width">
                                    <strong>Comentarios:</strong>
                                    <asp:TextBox ID="txtEditComentario" runat="server" TextMode="MultiLine" Rows="3"
                                        CssClass="comentario-textbox" Text='<%# Eval("p_comentario") %>'></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group acciones-participante">
                                <asp:LinkButton
                                    ID="btnEnviarWhatsapp"
                                    runat="server"
                                    Text="Enviar Whatsapp <img src='images/LogoWpp.png' alt='WhatsApp' class='icon-wpp' />"
                                    CssClass="btn-enviar-comentario"
                                    CommandName="EnviarWhatsapp"
                                    CommandArgument='<%# Eval("p_contador") %>' />

                                <asp:Button
                                    ID="btnGuardarCambiosItem"
                                    runat="server"
                                    Text="Guardar cambios"
                                    CssClass="btn-enviar-comentario"
                                    OnClick="btnGuardarCambios_Click" />

                                <asp:Button
                                    ID="btnEliminarParticipante"
                                    runat="server"
                                    Text="Eliminar"
                                    CssClass="btn-enviar-comentario btn-eliminar"
                                    CommandName="EliminarParticipante"
                                    CommandArgument='<%# Eval("p_contador") %>'
                                    OnClientClick="return confirm('¿Seguro que quieres eliminar este participante?');" />
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

    <div id="searchNav" class="search-nav" style="display: none;">
        <button type="button" class="search-nav-button" id="searchPrev">↑</button>
        <button type="button" class="search-nav-button" id="searchNext">↓</button>
    </div>

    <script src="Scripts/participantes-search.js"></script>
</body>
</html>