<%@ Page Language="C#" AutoEventWireup="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<%
    try
    {
        string source = "randomSource_" + Guid.NewGuid().ToString();
        if (!string.IsNullOrEmpty(Request.QueryString["source"]))
        {
            source = Request.QueryString["source"];
        }
        string identifier = "randomIdentifier_" + Guid.NewGuid().ToString();
        if (!string.IsNullOrEmpty(Request.QueryString["identifier"]))
        {
            identifier = Request.QueryString["identifier"];
        }
        AdminB.Feature.CivilDiscourse.xConnect.ContactX.CreateNewContact(source, identifier);
%>
    <h2>Contact created! Wubba lubba dub dub!</h2>
    <h4>Source: <%= source %></h4>
    <h4>Identifier: <%= identifier %></h4>
    <% }
    catch
    { %>
    <h2>Awww jeez, contact wasn't created.</h2>
    <% } %>
</body>
</html>
