using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Website.layouts;
using Website.Models;

namespace Website.Controllers
{
    public class CivilCommentsController : Controller
    {
        // GET: Default
        public ActionResult SubmitComment(CivilCommentsSectionViewModel model)
        {
            if (model.Comment == model.PreviousComment)
            {
                model.PromptAreYouSure = true;
                model.DisplayWarnings = true;
                return View("/Views/CivilCommentsSection.cshtml", model);
            }
            model.PromptAreYouSure = false;

            var comment = model.Comment;
            var dataSource = model.DatasourceItem;

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

            model.ReviewText = comment;

            if (warnings > 0)
            {
                model.PreviousComment = model.Comment;
                model.TotalWarnings += warnings;
                return View("/Views/CivilCommentsSection.cshtml", model);
            }
            else
            {
                CreateComment(model);
                return View("/Views/CivilCommentsSection.cshtml", new CivilCommentsSectionViewModel());
            }
        }

        public void CreateComment(CivilCommentsSectionViewModel model)
        {
            Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var dataSource = model.DatasourceItem;
            dataSource = masterDb.GetItem(dataSource.ID);
            var commentsFolder = dataSource.Axes.GetDescendants().FirstOrDefault(x => x.TemplateName == "Comments Folder");
            if (commentsFolder == null) return;
            TemplateItem template = masterDb.GetTemplate("{F0A8C4E9-FEE1-407E-99DB-A7F2D16024D1}");
            var newComment = commentsFolder.Add(String.Format("Comment {0}", DateTime.Now.ToString("g")).SanitizeToItemName(), template);

            var user = Sitecore.Context.User;
            var username = user == null ? "anon" : user.Name;

            if (newComment == null) return;
            newComment.Editing.BeginEdit();
            newComment.Fields["Comment"].Value = model.Comment;
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
                Value = val,
                Warning = (String.IsNullOrEmpty(item.Fields["Custom Warning"].Value) ? warning : item.Fields["Custom Warning"].Value),
                Color = GetSeverity(item)
            }).ToList();
        }

        public string GetSeverity(Item item)
        {
            ReferenceField severity = item.Fields["Severity Level"];
            var severityItem = severity.TargetItem;
            if (severityItem == null) return "";
            return severityItem.Fields["Warning Color"].Value;
        }
    }
}