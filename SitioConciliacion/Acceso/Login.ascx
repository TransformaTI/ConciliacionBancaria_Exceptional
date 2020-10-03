<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Login.ascx.cs" Inherits="Acceso_Login" %>
<div class="datos-estilo" style="text-align: left">
<%--    <div class="etiqueta" style="margin-bottom: 5px">
        Usuario</div>--%>
    <asp:TextBox ID="txtUsuario" runat="server" Visible="false" CssClass="cajaTexto" MaxLength="15" ToolTip="Escriba su nombre de usuario"
        Width="280px" placeholder="Insertar usuario"></asp:TextBox>
    <asp:RequiredFieldValidator ID="valUsuatio" runat="server" ControlToValidate="txtUsuario"
        Display="Dynamic" CssClass="etiqueta fg-color-rojo" ErrorMessage="Escriba su nombre de usuario.">*</asp:RequiredFieldValidator>
<%--    <div class="etiqueta" style="margin-bottom: 5px; margin-top: 10px">
        Clave</div>--%>
    <asp:TextBox ID="txtClave" runat="server" Visible="false" MaxLength="15" TextMode="Password" ToolTip="Escriba su clave de usuario."
        Width="280px" CssClass="cajaTexto" placeholder="Insertar clave"></asp:TextBox>
    <asp:RequiredFieldValidator ID="valClave" runat="server" ControlToValidate="txtClave"
        Display="Dynamic" CssClass="etiqueta fg-color-rojo" ErrorMessage="Escriba su clave de usuario.">*</asp:RequiredFieldValidator>
    <br />
    <br />
    <div style="width:300px">
        <asp:Button ID="btnEntrar" runat="server" Visible="false" CssClass="boton fg-color-blanco bg-color-verdeFuerte"
            Text="ENTRAR" ToolTip="Enviar datos de acceso." OnClick="btnEntrar_Click1" />
        <button type="button" onclick="window.location.href='http://40.118.227.251:8082/';" Class="boton fg-color-blanco bg-color-verdeFuerte">REGRESAR</button>
    </div>
</div>
