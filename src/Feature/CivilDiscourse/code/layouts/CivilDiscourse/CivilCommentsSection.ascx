<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CivilCommentsSection.ascx.cs" Inherits="AdminB.Feature.CivilDiscourse.layouts.CivilCommentsSection" %>

<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
<link rel="stylesheet" type="text/css" href="../css/styles.css" />
<script type="text/javascript" src="../scripts/commentbox.js"></script>

<asp:Literal runat="server" ID="IntroText"></asp:Literal>
<div class="comments-component">
    <div class="comments">
        <asp:Repeater runat="server" ID="CommentsRepeater" OnItemDataBound="CommentsRepeater_OnItemDataBound">
            <ItemTemplate>
                <div class="comment">
                    <div class="header">
                        <div class="username">
                            <asp:Literal runat="server" ID="CommentUserName"></asp:Literal>:
                        </div>
                        <div class="date">
                            <asp:Literal runat="server" ID="CommentDate"></asp:Literal>
                        </div>
                    </div>
                    <div class="body">
                        <asp:Literal runat="server" ID="CommentBody"></asp:Literal>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>


    <asp:PlaceHolder ID="ReviewCommentBox" runat="server" Visible="false">
        
        <div class="review-box">
            <h4 class="review-heading">We detected some language in your comment that is not conducive to a productive discussion. Please review your comment and consider changes the words and phrases we've highlighted. However over highlighted words for helpful tips and suggestions</h4>
            <asp:Literal runat="server" ID="ReviewCommentText"></asp:Literal>
        </div>
    </asp:PlaceHolder>


    <form id="comment-box">
        <div class="comment-box">
            <asp:TextBox runat="server" TextMode="MultiLine" ID="Comment" CssClass="comment-txt-box" Columns="50" Rows="10"></asp:TextBox>
            <a href="javascript:void(0);" class="submit-button">Submit</a>
            <asp:Button runat="server" ID="SubmitButton" OnClick="SubmitButton_Click" CssClass="submit-btn-hidden hidden" />

        </div>
    </form>
    <span class="error-message">You haven't typed anything!</span>
    <asp:PlaceHolder runat="server" ID="AreYouSure" Visible="false">
        <div class="overlay">
            <div class="are-you-sure">
                <a class="close-overlay close">X</a>
                <span>It looks like you haven't made any changes. Are you sure you want to submit this comment?</span>
                <asp:Button runat="server" Text="Yes, I'm Really Sure" OnClick="YesPleaseSubmitThisAwfulComment"></asp:Button>
                <a class="close-overlay-btn close" href="javascript:void(0)">No, I'll revise it</a>
            </div>
        </div>
    </asp:PlaceHolder>
    
    <div style="display:none">
        <asp:Literal runat="server" ID="PreviousComment"></asp:Literal>
        <asp:Literal runat="server" ID="TotalWarnings"></asp:Literal>
    </div>
</div>

