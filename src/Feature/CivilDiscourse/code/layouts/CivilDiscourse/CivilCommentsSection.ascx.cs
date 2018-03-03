using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Publishing;
using Sitecore.Visualization;
using Sitecore.Web.UI.XslControls;
using Literal = System.Web.UI.WebControls.Literal;

namespace Website.layouts
{
    public partial class CivilCommentsSection : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReviewCommentBox.Visible = false;

            var dataSource = GetDataSourceItem();

            if (dataSource == null) return;

            IntroText.Text = dataSource.Fields["Intro Text"].Value;

            DisplayComments();
        }

        public void DisplayComments()
        {
            var dataSource = GetDataSourceItem();
            if (dataSource == null) return;

            var commentsFolder = dataSource.Axes.GetDescendants().FirstOrDefault(x => x.TemplateName == "Comments Folder");
            if (commentsFolder == null) return;
            var comments =
                commentsFolder.Axes.GetDescendants()
                    .Where(x => x.TemplateName == "Comment")
                    .OrderBy(x => x.Created);

            CommentsRepeater.DataSource = comments;
            CommentsRepeater.DataBind();
        }

        protected void SubmitBtn_OnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public Item GetDataSourceItem()
        {
            var sublayout = this.Parent as Sitecore.Web.UI.WebControls.Sublayout;
            if (sublayout != null)
            {
                Guid dataSourceId;
                Sitecore.Data.Items.Item dataSource;
                if (Guid.TryParse(sublayout.DataSource, out dataSourceId))
                {
                    dataSource = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(dataSourceId));
                }
                else
                {
                    dataSource = Sitecore.Context.Database.GetItem(sublayout.DataSource);
                }
                return dataSource;
            }
            return null;
        }

        protected void CommentsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Sitecore.Data.Items.Item item = e.Item.DataItem as Sitecore.Data.Items.Item;
            Literal litUsername = (Literal)e.Item.FindControl("CommentUserName");
            Literal litDate = (Literal)e.Item.FindControl("CommentDate");
            Literal litBody = (Literal)e.Item.FindControl("CommentBody");

            litUsername.Text = item.Fields["Username"].Value.Replace("sitecore\\", "");
            litBody.Text = item.Fields["Comment"].Value;
            litDate.Text = item.Created.ToString("g");

        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            if (Comment.Text == PreviousComment.Text)
            {
                AreYouSure.Visible = true;
                ReviewCommentBox.Visible = true;
                return;
            }
            AreYouSure.Visible = false;

            var comment = Comment.Text;

            var dataSource = GetDataSourceItem();

            List<Word> words = new List<Word>();
            Sitecore.Data.Fields.MultilistField flaggedWords = dataSource.Fields["Flagged Words"];
            Sitecore.Data.Fields.MultilistField wordGroups = dataSource.Fields["Flagged Word Groups"];

            var cooldownTime = dataSource.Fields["Cooldown Minutes"].Value;

            foreach (var wordItem in flaggedWords.GetItems())
            {
                words.AddRange(GetWords(wordItem));
            }

            foreach (var wordGroup in wordGroups.GetItems())
            {
                var warning = wordGroup.Fields["Warning Text"].Value;
                Sitecore.Data.Fields.MultilistField wordsField = wordGroup.Fields["Words"];
                foreach (var wordItem in wordsField.GetItems())
                {
                    words.AddRange(GetWords(wordItem, warning));
                }
            }

            var warnings = 0;

            foreach (var word in words) // scan comment for the word
            {
                warnings += comment.CountMatches(word.Value);

                var regex = new Regex(word.Value, RegexOptions.IgnoreCase);

                string formattedWord = String.Format("<span class='warning-word' style='background-color:{2}'>{0}<span class='tool-tip'>{1}</span></span>", word.Value, word.Warning, word.Color);

                comment = regex.Replace(comment, formattedWord);
            }

            ReviewCommentText.Text = comment;

            if (warnings > 0)
            {
                PreviousComment.Text = Comment.Text;
                //TotalWarnings.Text = (warnings + Int32.Parse(TotalWarnings.Text)).ToString();
                ReviewCommentBox.Visible = true;
            }
            else
            {
                SubmitComment();
            }       
        }

        public void SubmitComment()
        {
            ReviewCommentBox.Visible = false;
            AreYouSure.Visible = false;
            CreateComment(Comment.Text);
            Comment.Text = "";
            PreviousComment.Text = "";
            DisplayComments();
        }

        public void CreateComment(string comment)
        {
            Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var dataSource = GetDataSourceItem();
            dataSource = masterDb.GetItem(dataSource.ID);
            var commentsFolder = dataSource.Axes.GetDescendants().FirstOrDefault(x => x.TemplateName == "Comments Folder");
            if (commentsFolder == null) return;
            TemplateItem template = masterDb.GetTemplate("{F0A8C4E9-FEE1-407E-99DB-A7F2D16024D1}");
            var newComment = commentsFolder.Add(String.Format("Comment {0}", DateTime.Now.ToString("g")).SanitizeToItemName(), template);

            var user = Sitecore.Context.User;
            var username = user == null ? "anon" : user.Name;

            if (newComment == null) return;
            newComment.Editing.BeginEdit();
            newComment.Fields["Comment"].Value = comment;
            newComment.Fields["Username"].Value = username;
            newComment.Editing.EndEdit();

            Sitecore.Publishing.PublishOptions publishOptions =
            new Sitecore.Publishing.PublishOptions(newComment.Database,
                                           Database.GetDatabase("web"),
                                           Sitecore.Publishing.PublishMode.SingleItem,
                                           newComment.Language,
                                           System.DateTime.Now);  // Create a publisher with the publishoptions
            Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions);

            publisher.Options.RootItem = newComment;
            publisher.Publish();
        }

        public List<Word> GetWords(Item item, string warning = "")
        {
            var values = item.Fields["Word"].Value.Split(',').Where(x => !String.IsNullOrEmpty(x)).Select(x => x.Trim());

            return values.Select(val => new Word()
            {
                Value = val, Warning = (String.IsNullOrEmpty(item.Fields["Custom Warning"].Value) ? warning : item.Fields["Custom Warning"].Value), Color = GetSeverity(item)
            }).ToList();
        }

        public string GetSeverity(Item item)
        {
            ReferenceField severity = item.Fields["Severity Level"];
            var severityItem = severity.TargetItem;
            if (severityItem == null) return "";
            return severityItem.Fields["Warning Color"].Value;
        }

        protected void YesPleaseSubmitThisAwfulComment(object sender, EventArgs e)
        {
            SubmitComment();
        }
    }

    public class Word
    {
        public string Value { get; set; }
        public string Warning { get; set; }
        public string Color { get; set; }
    }

    public static class StringExtension
    {
        public static string SanitizeToItemName(this string possibleName)
        {
            return SanitizeToItemName(possibleName, Sitecore.Configuration.Settings.InvalidItemNameChars);
        }

        public static string SanitizeToItemName(this string possibleName, char[] invalidCharacters)
        {
            return string.Concat(possibleName.Trim().Split(invalidCharacters));
        }
        public static int CountMatches(this string source, string searchText)
        {

            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(searchText))
                return 0;

            source = source.ToLower();
            searchText = searchText.ToLower();

            int counter = 0;
            int startIndex = -1;
            while ((startIndex = (source.IndexOf(searchText, startIndex + 1))) != -1)
                counter++;
            return counter;
        }

        public static string CapitalizeFirst(this string s)
        {
            bool IsNewSentense = true;
            var result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (IsNewSentense && char.IsLetter(s[i]))
                {
                    result.Append(char.ToUpper(s[i]));
                    IsNewSentense = false;
                }
                else
                    result.Append(s[i]);

                if (s[i] == '!' || s[i] == '?' || s[i] == '.')
                {
                    IsNewSentense = true;
                }
            }

            return result.ToString();
        }
    }
}