<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetAllContactIDs.aspx.cs"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
        <div>
            <h3>All Contact IDs</h3>
            <p>
                <%= (new AdminB.Feature.CivilDiscourse.xConnect.Contacts()).GetAllContacts() %>
            </p>
        </div>
</body>
</html>
