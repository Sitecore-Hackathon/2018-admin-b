<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Analytics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<sc:VisitorIdentification runat="server"/>
<%
    try
    {string comment = "randomComment_" + Guid.NewGuid().ToString();
        if (!string.IsNullOrEmpty(Request.QueryString["comment"]))
        {
            comment = Request.QueryString["comment"];
        }

        int warningCount = 12;
        if (!string.IsNullOrEmpty(Request.QueryString["warningCount"]))
        {
            warningCount = Int32.Parse(Request.QueryString["warningCount"]);
        }

        var comments = new AdminB.Feature.CivilDiscourse.xConnect.Comments();
        comments.SubmitComment(comment, warningCount);
%>
    <h2>Comment created! Wubba lubba dub dub!</h2>
    <h4>Comment: <%= comment %></h4>
    <h4>Warning Count: <%= warningCount %></h4>
<% }
    catch (Exception ex)
    { %>
    <h2>Awww jeez, comment wasn't created.</h2>
    <h4><%= ex.Message %></h4>
    <h4><%= ex.StackTrace %></h4>
<% } %>
</body>
</html>
