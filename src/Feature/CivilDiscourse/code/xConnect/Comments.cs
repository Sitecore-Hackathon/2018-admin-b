using System;
using System.Linq;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;

namespace AdminB.Feature.CivilDiscourse.xConnect
{
    public class Comments
    {
        public void ExampleSync()
        { //https://doc.sitecore.net/developers/xp/xconnect/xconnect-client-api/search/interactions/index.html#search-by-event-definition-id
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var someGoalId = Guid.Parse("0565e9b0-936c-4594-8cc3-5fcace3918ed"); // Replace with real goal ID

                    IAsyncQueryable<Sitecore.XConnect.Interaction> queryable = client.Interactions
                        .Where(x => x.Events.Any(y => y.DefinitionId == someGoalId));

                    var enumerable = queryable.GetBatchEnumeratorSync(20);

                    while (enumerable.MoveNext())
                    {
                        var interactionBatch = enumerable.Current; // Batch of <= 20 interactions

                        foreach (var interaction in interactionBatch)
                        {
                            var matchingGoals = interaction.Events.OfType<Goal>().Where(x => x.DefinitionId == someGoalId).ToList();
                        }
                    }

                }
                catch (XdbExecutionException ex)
                {
                    // Handle exception
                }
            }
        }
    }
}
