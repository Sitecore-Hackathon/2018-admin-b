using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Analytics;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Operations;

namespace AdminB.Feature.CivilDiscourse.xConnect
{
    public class ContactX
    {
        // Sync example
        public static string GetAllContacts()
        {
            using (Sitecore.XConnect.Client.XConnectClient client =
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

        public static void CreateNewContact()
        {
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var firstContact = new Sitecore.XConnect.Contact();
                    client.AddContact(firstContact); // Extension found in Sitecore.XConnect.Operations

                    // Submits the batch, which contains two operations
                    client.Submit();
                }
                catch (XdbExecutionException ex)
                {
                    // Manage exception
                }
            }
        }

        public static void CreateNewContact(string source, string identifier)
        {
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var firstContact = new Sitecore.XConnect.Contact(
                        new Sitecore.XConnect.ContactIdentifier(source, identifier, ContactIdentifierType.Known)
                        );

                    client.AddContact(firstContact); // Extension found in Sitecore.XConnect.Operations

                    // Submits the batch, which contains two operations
                    client.Submit();
                }
                catch (XdbExecutionException ex)
                {
                    // Manage exception
                }
            }
        }

        public void SubmitComment()
        {
            using (Sitecore.XConnect.Client.XConnectClient client =
                Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {

                    var interactionIdOne = Guid.Parse("DA2DA5F0-4348-E611-82E7-34E6D7117DCB");
                    var interactionIdTwo = Guid.Parse("{B9814105-1F45-E611-82E6-34E6D7117DCB}");

                    // Contact reference from ID
                    var contactRef = new ContactReference(interactionIdOne);
                    Sitecore.XConnect.InteractionReference interactionRef =
                        new Sitecore.XConnect.InteractionReference(contactRef, interactionIdTwo);

                    // Contact reference from identifier
                    var identifiedContactRef = new IdentifiedContactReference("twitter", "myrtlesitecore");
                    Sitecore.XConnect.InteractionReference secondInteractionRef =
                        new Sitecore.XConnect.InteractionReference(identifiedContactRef,
                            Guid.Parse("E6067926-1F45-E611-82E6-34E6D7117DCB"));

                    var references = new List<Sitecore.XConnect.InteractionReference>()
                    {
                        interactionRef,
                        secondInteractionRef
                    };

                    IReadOnlyCollection<IEntityLookupResult<Interaction>> interactions =
                        client.Get<Interaction>(references, new Sitecore.XConnect.InteractionExpandOptions() { });
                }
                catch (Exception ex)
                {
                    // Manage exceptions
                }
            }
        }

        public async void SetContactFacet()
        {
            using (Sitecore.XConnect.Client.XConnectClient client =
                Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                {
                    try
                    {
                        Sitecore.Analytics.Tracking.Contact thisContact = Tracker.Current.Contact;

                        Contact contact = new Contact(new ContactIdentifier("twitter", "myrtlesitecore",
                            ContactIdentifierType.Known));

                        client.AddContact(contact);

                        // Facet with a reference object, key is specified
                        PersonalInformation personalInfoFacet = new PersonalInformation()
                        {
                            FirstName = "Myrtle",
                            LastName = "McSitecore"
                        };

                        FacetReference reference = new FacetReference(contact, PersonalInformation.DefaultFacetKey);

                        client.SetFacet(reference, personalInfoFacet);

                        // Facet without a reference, using default key
                        EmailAddressList emails =
                            new EmailAddressList(new EmailAddress("myrtle@test.test", true), "Home");

                        client.SetFacet(contact, emails);

                        // Facet without a reference, key is specified

                        AddressList addresses =
                            new AddressList(
                                new Address()
                                {
                                    AddressLine1 = "Cool Street 12",
                                    City = "Sitecore City",
                                    PostalCode = "ABC 123"
                                }, "Home");

                        client.SetFacet(contact, AddressList.DefaultFacetKey, addresses);

                        // Submit operations as batch
                        client.Submit();
                    }
                    catch (XdbExecutionException ex)
                    {

                    }
                }
            }
        }
    }
}
