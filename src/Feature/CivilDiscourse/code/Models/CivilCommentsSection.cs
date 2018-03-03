using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace AdminB.Feature.CivilDiscourse.Models
{
    public class CivilCommentsSectionViewModel
    {
        public List<Item> Comments { get; set; } 

        public String IntroText { get; set; }

        public Item DatasourceItem { get; set; }

        public string Comment { get; set; }

        public string PreviousComment { get; set; }

        public bool DisplayWarnings { get; set; }

        public bool PromptAreYouSure { get; set; }

        public string ReviewText { get; set; }

        public int TotalWarnings { get; set; }

        public bool ClearTextbox { get; set; }

        public string SubmitShittyComment { get; set; }

        public int Cooldown { get; set; }

        public CivilCommentsSectionViewModel()
        {
            var dataSourceId = "{6BFAC490-3F0A-406F-88D6-5619CB008EBB}";    // global settings item
            var dataSource = Sitecore.Context.Database.GetItem(dataSourceId);
            DatasourceItem = dataSource;
            if (dataSource == null) return;

            var commentsFolder = Sitecore.Context.Database.GetItem("{A0660FA6-D500-453A-A807-4FFE7997F83C}");
            if (commentsFolder == null) return;
            var comments =
                commentsFolder.Axes.GetDescendants()
                    .Where(x => x.TemplateName == "Comment")
                    .OrderBy(x => x.Created);

            Comments = comments.ToList();
            IntroText = dataSource.Fields["Intro Text"].Value;
        }


    }
}