using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Website.Models
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

        public CivilCommentsSectionViewModel()
        {
            var dataSourceId = RenderingContext.CurrentOrNull.Rendering.DataSource;
            var dataSource = Sitecore.Context.Database.GetItem(dataSourceId);
            DatasourceItem = dataSource;
            if (dataSource == null) return;

            var commentsFolder = dataSource.Axes.GetDescendants().FirstOrDefault(x => x.TemplateName == "Comments Folder");
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