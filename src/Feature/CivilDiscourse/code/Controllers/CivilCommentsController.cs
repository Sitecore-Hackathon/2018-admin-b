using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AdminB.Feature.CivilDiscourse.layouts;
using AdminB.Feature.CivilDiscourse.Models;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace AdminB.Feature.CivilDiscourse.Controllers
{
    public class CivilCommentsController : Controller
    {
        public ActionResult Index()
        {
            var model = new CivilCommentsSectionViewModel();
            return View("/Views/CivilCommentsSection.cshtml", model);
        }

        // GET: Default
        public ActionResult SubmitComment(CivilCommentsSectionViewModel model)
        {
            model.ClearTextbox = false;
            if (model.SubmitShittyComment == "true")
            {
                CreateComment(model);
                return View("/Views/CivilCommentsSection.cshtml", new CivilCommentsSectionViewModel()
                {
                    ClearTextbox = true
                });
            }
                       
            model.PromptAreYouSure = false;

            var comment = model.Comment;
            var dataSource = model.DatasourceItem;

            List<Word> words = new List<Word>();
            Sitecore.Data.Fields.MultilistField flaggedWords = dataSource.Fields["Flagged Words"];
            Sitecore.Data.Fields.MultilistField wordGroups = dataSource.Fields["Flagged Word Groups"];

            var cooldownTime = dataSource.Fields["Cooldown"].Value;

            int cooldown = 5000;
            if (!String.IsNullOrEmpty(cooldownTime))
            {
                if (int.TryParse(cooldownTime, out cooldown))
                {
                    model.Cooldown = cooldown;
                }
            }

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

            if (model.Comment == model.PreviousComment)
            {
                model.PromptAreYouSure = true;
                model.DisplayWarnings = true;
                model.PreviousComment = model.Comment;
                return View("/Views/CivilCommentsSection.cshtml", model);
            }

            if (warnings > 0)
            {
                model.DisplayWarnings = true;
                model.PreviousComment = model.Comment;
                model.TotalWarnings += warnings;
                return View("/Views/CivilCommentsSection.cshtml", model);
            }
            else
            {
                CreateComment(model);
                return View("/Views/CivilCommentsSection.cshtml", new CivilCommentsSectionViewModel()
                {
                    ClearTextbox = true
                });
            }
        }

        public void CreateComment(CivilCommentsSectionViewModel model)
        {
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                var dataSource = model.DatasourceItem;
                dataSource = masterDb.GetItem(dataSource.ID);
                var commentsFolder = masterDb.GetItem("{A0660FA6-D500-453A-A807-4FFE7997F83C}");
                if (commentsFolder == null) return;
                TemplateItem template = masterDb.GetTemplate("{F0A8C4E9-FEE1-407E-99DB-A7F2D16024D1}");
                var newComment =
                    commentsFolder.Add(String.Format("Comment {0}", DateTime.Now.ToString("g")).SanitizeToItemName(),
                        template);

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
                        System.DateTime.Now); // Create a publisher with the publishoptions
                Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions);

                publisher.Options.RootItem = newComment;
                publisher.Publish();
            }
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