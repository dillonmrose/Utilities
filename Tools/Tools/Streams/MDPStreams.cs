using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Geospatial.Data.Gateway;
using Microsoft.Geospatial.Data.Gateway.ObjectModel;
using Microsoft.Geospatial.Data.Gateway.OntologyModel;

namespace Tools
{
    class MDPStreams
    {
        public static async Task query(string[] args)
        {
            //This is all in the "Microsoft.Geospatial.Data.Gateway" Namespace
            //using Microsoft.Graph;
            //using Microsoft.TeamFoundation.Client;
            //using Microsoft.TeamFoundation.WorkItemTracking.Client;

            //Your Client id can be obtained from: https://streamsgatewaywebtools.azurewebsites.net/ClientIdManagement treat it like a password.
            //You may need to request read access to a branch, this can be done from the same portal.
            var connectionInfo = new ConnectionInfo("mdpgw-prod.westus.cloudapp.azure.com", "AgUHB2Rpcm9zZQkdHQEFAAAAAAAJeXdHUyldHEKwxj8_IrKxozBxqwsA ", "Boilerplate App");
            var gateway = new StreamsGateway(connectionInfo);

            BranchView branchView = await gateway.GetBranchView("main");

            // you'll need the onotology (specifically the lookups as you'll see below, to help with filtering)
            Ontology ontology = await branchView.GetOntology();

            var enGbLocal = ontology.Lookups.LocaleKeys["en-gb"];
            var roadEntities = ontology.Lookups.EntityTypeKeys["Street"];
            var entityFilter = new EntityFilter()
            {
                Locales = new List<LocaleKey>() { enGbLocal },
                EntityTypes = new List<EntityTypeKey>() { roadEntities },
                // lots of potentail areas to filter on.
            };
            
            //This has a few overrides. Just using entityFilter as example.
            ResultSet<Entity> entities = branchView.GetEntities(entityFilter, DataComponents.FullEntities);
            var entityType = ontology.Lookups.PrimitivePropertyKeys["AddressRange"];

            foreach (Entity entity in entities)
            {
                Console.WriteLine("HERE");
            }

            //You can also Query primitives.
            var primitiveFilter = new PrimitiveFilter()
            {
                //Various Primitive Filters.
            };
            var primitives = branchView.GetPrimitives(primitiveFilter,
                DataComponents.Entities | DataComponents.Primitives | DataComponents.PrimitiveProperties);
            Console.WriteLine("Done Final");
        }
    }
}
