using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;

namespace AdminB.Feature.CivilDiscourse.xConnect
{
    /// <summary>
    /// Provides APIs for interacting with comments via xConnect. 
    /// </summary>
    public class Comments
    {
        public const string CommentPrefix = "CivilDiscourse:comment:";
        public const string WarningPrefix = ":warningCount:";

        /// <summary>
        /// Submit the details of a new attempted or actual comment for the current user.
        /// </summary>
        /// <param name="comment">The actual text of the comment</param>
        /// <param name="warningCount">The number of warnings in the text that were shown to the user.</param>
        /// <remarks>Sorry, this is such a lame hack. If we were to describe how lame it is, our own CivilDiscourse module would kick us off the Internet.
        /// 
        /// Sorry. -admin/b</remarks>
        public void SubmitComment(string comment, int warningCount)
        {
            try
            {
                if (Tracker.MarketingDefinitions == null || Tracker.MarketingDefinitions.PageEvents == null)
                {
                    throw new Exception("Something in tracker is null :(");
                }

                var searchEvent = Tracker.MarketingDefinitions.PageEvents[AnalyticsIds.SearchEvent.Guid];

                // Sorry not sorry... WHAT HAPPENS IN HACKATHON STAYS IN HACKATHON
                Tracker.Current.CurrentPage.Register(new PageEventData(searchEvent.Alias, searchEvent.Id)
                {
                    Data = $"{CommentPrefix}{comment}{WarningPrefix}{warningCount.ToString()}",
                });
            }
            catch (XdbExecutionException ex)
            {
                Sitecore.Diagnostics.Log.Error("Aww jeez, something went wrong creating a comment :(", ex, this);
            }
        }

        /// <summary>
        /// Get the warning words that are violated the most often on the site.
        /// </summary>
        public Dictionary<string, int> GetMostViolatedWords()
        {
            // Create a custom event for the contact that has the comment text and the warning count
            using (XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    // Not yet implemented :/
                }
                catch (XdbExecutionException ex)
                {
                    Sitecore.Diagnostics.Log.Error("Aww jeez, something went wrong getting list of most violated words :(", ex, this);
                }
                return new Dictionary<string, int>();
            }
        }
    }
}
