using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Operations;

namespace CivilDiscorse.TextAnalyzer.xConnect
{
    public class Contact
    {
        // Async example
        public async void Example()
        {
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    int count = await client.Contacts.Where(c => c.Identifiers.Any(t => t.IdentifierType == Sitecore.XConnect.ContactIdentifierType.Known)).Count();
                }
                catch (XdbExecutionException ex)
                {
                    // Handle exception
                }
            }
        }

        // Sync example
        public void ExampleSync()
        {
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    // There is no synchronous extension for Count - use SuspendContextLock instead
                    int count = Sitecore.XConnect.Client.XConnectSynchronousExtensions.SuspendContextLock(client.Contacts.Where(c => c.Identifiers.Any(t => t.IdentifierType == Sitecore.XConnect.ContactIdentifierType.Known)).Count);
                }
                catch (XdbExecutionException ex)
                {
                    // Handle exception
                }
            }
        }

        public void SubmitComment()
        {
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {

                    var interactionIdOne = Guid.Parse("DA2DA5F0-4348-E611-82E7-34E6D7117DCB");
                    var interactionIdTwo = Guid.Parse("{B9814105-1F45-E611-82E6-34E6D7117DCB}");

                    // Contact reference from ID
                    var contactRef = new ContactReference(interactionIdOne);
                    Sitecore.XConnect.InteractionReference interactionRef = new Sitecore.XConnect.InteractionReference(contactRef, interactionIdTwo);

                    // Contact reference from identifier
                    var identifiedContactRef = new IdentifiedContactReference("twitter", "myrtlesitecore");
                    Sitecore.XConnect.InteractionReference secondInteractionRef = new Sitecore.XConnect.InteractionReference(identifiedContactRef, Guid.Parse("E6067926-1F45-E611-82E6-34E6D7117DCB"));

                    var references = new List<Sitecore.XConnect.InteractionReference>()
                    {
                        interactionRef, secondInteractionRef
                    };

                    IReadOnlyCollection<IEntityLookupResult<Interaction>> interactions = client.Get<Interaction>(references, new Sitecore.XConnect.InteractionExpandOptions() { });
                }
                catch (Exception ex)
                {
                    // Manage exceptions
                }
            }
        }
        }
    }
}
