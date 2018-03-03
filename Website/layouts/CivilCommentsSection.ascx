<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CivilCommentsSection.ascx.cs" Inherits="Website.layouts.CivilCommentsSection" %>

<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
<link rel="stylesheet" type="text/css" href="../css/styles.css" />
<script type="text/javascript" src="../scripts/commentbox.js"></script>

<div class="comments-component">
    <div class="comments">
<asp:Repeater runat="server" ID="CommentsRepeater" OnItemDataBound="CommentsRepeater_OnItemDataBound">
    <ItemTemplate>
        <div class="comment">
            <div class="header">
                <asp:Literal runat="server" ID="CommentUserName"></asp:Literal>
                <asp:Literal runat="server" ID="CommentDate"></asp:Literal>
            </div>
            <div class="body">
                <asp:Literal runat="server" ID="CommentBody"></asp:Literal>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
        </div>

    <asp:PlaceHolder ID="ReviewCommentBox" runat="server" Visible="false">
        <div class="review-box"></div>
    </asp:PlaceHolder>

<form id="comment-box">
    <asp:TextBox runat="server" ID="Comment" CssClass="comment-txt-box"></asp:TextBox>
    <a href="javascript:void(0);" class="submit-button" >Submit</a>
    <asp:Button runat="server" ID="SubmitButton" OnClick="SubmitButton_Click" CssClass="submit-btn-hidden hidden"/>
</form>
    <span class="error-message">You haven't typed anything!</span>
    </div>