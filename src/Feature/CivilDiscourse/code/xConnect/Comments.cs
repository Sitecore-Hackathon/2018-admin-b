using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;

namespace AdminB.Feature.CivilDiscourse.xConnect
{
    /// <summary>
    /// Provides APIs for interacting with comments via xConnect. 
    /// </summary>
    public class Comments
    {
        /// <summary>
        /// Submit the details of a new attempted or actual comment.
        /// </summary>
        /// <param name="contact">The xConnect contact making the comment.</param>
        /// <param name="comment">The actual text of the comment</param>
        /// <param name="warningCount">The number of warnings in the text that were shown to the user.</param>
        /// <remarks></remarks>
        public void SubmitComment(Contact contact, string comment, int warningCount)
        {
            // Create a custom event for the contact that has the comment text and the warning count
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {

                }
                catch (XdbExecutionException ex)
                {
                    // Handle exception
                }
            }
        }

        /// <summary>
        /// Get the warning words that are violated the most often on the site.
        /// </summary>
        public void GetMostViolatedWords()
        {
            // Create a custom event for the contact that has the comment text and the warning count
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {

                }
                catch (XdbExecutionException ex)
                {
                    // Handle exception
                }
            }
        }
    }
}
