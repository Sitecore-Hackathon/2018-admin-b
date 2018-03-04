using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sitecore.Analytics;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Operations;

namespace AdminB.Feature.CivilDiscourse.xConnect
{
    /// <summary>
    /// Provides APIs for interacting with contacts via xConnect. 
    /// </summary>
    public class Contacts
    {
        /// <summary>
        /// Get all contacts in xDB and their identifiers.
        /// </summary>
        /// <returns>Returns an HTML-ready list of all contacts in xDB and any identifiers associated to the account.</returns>
        /// <remarks>This is primarily a debugging method.</remarks>
        public string GetAllContacts()
        {
            using (XConnectClient client =
                Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var contactSB = new StringBuilder();

                    foreach (Contact contact in client.Contacts.AsEnumerable())
                    {
                        contactSB.Append("contact ID = ");
                        contactSB.Append(contact.Id.ToString());
                        foreach (var id in contact.Identifiers)
                        {
                            contactSB.Append(", identifier = ");
                            contactSB.Append(id.Identifier);
                            contactSB.Append(", source = ");
                            contactSB.Append(id.Source);
                            contactSB.Append(", identifier type = ");
                            contactSB.Append(id.IdentifierType.ToString());
                        }

                        contactSB.Append("  ");

                        contactSB.AppendLine("<br /><br />");

                        contactSB.AppendLine();
                    }

                    return contactSB.ToString();
                }
                catch (XdbExecutionException ex)
                {
                    //oh fuck
                    return ex.Message + ex.StackTrace;
                }
            }
        }

        /// <summary>
        /// Create a new contact in xDB with the supplied identifier details.
        /// </summary>
        /// <param name="source">The source of the contact (e.g. "Sitecore", "XboxLive").</param>
        /// <param name="identifier">An identifier for the contact (e.g. a username, email address, or social security number). </param>
        /// <returns>The contact that was created, or null if we can't create the contact.</returns>
        public Contact CreateNewContact(string source, string identifier)
        {
            using (XConnectClient client =
                Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var firstContact = new Contact(
                        new ContactIdentifier(source, identifier, ContactIdentifierType.Known)
                    );

                    client.AddContact(firstContact); // Extension found in Sitecore.XConnect.Operations

                    // Submits the batch, which contains two operations
                    client.Submit();

                    return firstContact;
                }
                catch (XdbExecutionException ex)
                {
                    // Awww jeez, this is bad
                    Sitecore.Diagnostics.Log.Error("Tried to create a new contact, but we failed :(", ex, this);

                    return null;
                }
            }
        }

        /// <summary>
        /// Get a contact with the supplied identifier details, or create a contact if no such contact exists.
        /// </summary>
        /// <param name="source">The source of the contact (e.g. "Sitecore", "EquifaxBreachData").</param>
        /// <param name="identifier">An identifier for the contact (e.g. a username, email address, or ICQ number). </param>
        /// <returns>The contact that was found or created, or null if we can't find or create the contact.</returns>
        public Contact GetOrCreateContact(string source, string identifier)
        {
            var reference = new IdentifiedContactReference(source, identifier);

            using (XConnectClient client =
                Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    Contact contact = client.Get<Contact>(reference, new ContactExpandOptions() { });

                    if (contact == null)
                    {
                        contact = CreateNewContact(source, identifier);
                    }

                    if (contact /* is still */ == null)
                    {
                        throw new Exception("Error when creating or getting contact :(");
                    }

                    return contact;
                }
                catch (Exception ex)
                {
                    // Awww jeez, this is bad
                    Sitecore.Diagnostics.Log.Error("Tried to create or get a new contact, but we failed :(", ex, this);

                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a list of the worst commenters.
        /// </summary>
        /// <returns>A list of commenters sorted by percentage of warnings received over total comments.</returns>
        public Dictionary<string, CommentsAndWarnings> GetWorstCommenters()
        {
            //var contacts = new List<ScoredContact>();
            var contacts = new Dictionary<string, CommentsAndWarnings>();
            using (XConnectClient client =
                Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    IAsyncQueryable<Interaction> queryable = client.Interactions
                        .Where(x => x.Events.OfType<Event>().Any());

                    var enumerable = queryable.GetBatchEnumeratorSync(20);

                    while (enumerable.MoveNext())
                    {
                        var interactionBatch = enumerable.Current;

                        foreach (var interaction in interactionBatch)
                        {
                            // if event is a search event and if it begins with our magic string, fucking parse that shit
                            var searchEvents = interaction.Events.OfType<SearchEvent>();
                            foreach (var searchEvent in searchEvents)
                            {
                                // sorry
                                if (searchEvent.Data.StartsWith(Comments.CommentPrefix))
                                {
                                    // Congratulations! It's a comment!
                                    string s1 = searchEvent.Data.Substring(Comments.CommentPrefix.Length);
                                    int index = s1.IndexOf(Comments.WarningPrefix);
                                    string comment = s1.Substring(0, index); //we don't need this in v1
                                    string warningCountString = s1.Substring(index + Comments.WarningPrefix.Length);
                                    int warningCount = 0;
                                    Int32.TryParse(warningCountString, out warningCount);

                                    IEntityReference<Contact> reference = interaction.Contact;

                                    Contact contact = client.Get(reference, new ExpandOptions());
                                    string identifier = GetIdentifier(contact);
                                    SaveInformation(contacts, identifier, warningCount);


                                    // not sorry
                                }
                            }
                        }
                    }


                    return contacts;
                }


                catch (Exception ex)
                {
                    // Awww jeez, this is bad
                    Sitecore.Diagnostics.Log.Error("Tried to create the list of worst commenters, but we failed :(",
                        ex, this);

                    return null;
                }
            }
        }

        private string GetIdentifier(Contact contact)
        {
            return contact.Identifiers.FirstOrDefault()?.Identifier ?? "";
        }

        private void SaveInformation(Dictionary<string, CommentsAndWarnings> contacts, string identifier,
            int warningCount)
        {
            if (contacts.ContainsKey(identifier))
            {
                contacts[identifier].Comments++;
                contacts[identifier].Warnings += warningCount;
            }
            else
            {
                contacts.Add(identifier, new CommentsAndWarnings {Comments = 1, Warnings = warningCount});
            }
        }

        /// <summary>
        /// A contact annotated with comment and warning counts.
        /// </summary>
        public class ScoredContact
        {
            public Contact Contact { get; set; }
            public int NumberOfComments { get; set; }
            public int NumberOfWarnings { get; set; }

            public string WarningsPerComment()
            {
                var pct = NumberOfWarnings * 100.0 / NumberOfComments;
                return pct.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    public class CommentsAndWarnings
    {
        public int Comments { get; set; }
        public int Warnings { get; set; }
    }
}
