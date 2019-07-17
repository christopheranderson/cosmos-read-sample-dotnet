using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosReadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                run().Wait();
            } catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            Console.Write("Press key to exit...");
            Console.ReadKey(); // wait until user input to close console window because I'm laaaaazy
        }

        private static async Task run(bool secondTry = false)
        {
            // Edit these...
            string endpoint = "";
            string key = "";
            string databaseId = "";
            string collectionId = "";
            string documentId = "";
            string partitionKeyValue = "";

            DocumentClient client = new DocumentClient(new Uri(endpoint), key, new ConnectionPolicy() { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });

            try
            {
                Console.WriteLine("... Trying to read document (id, pk): (" + documentId + ", " + partitionKeyValue + ")");
                ResourceResponse <Document> doc = await client.ReadDocumentAsync(
                    "/dbs/" + databaseId + "/colls/" + collectionId + "/docs/" + documentId,
                    new RequestOptions() { PartitionKey = new PartitionKey(partitionKeyValue) }
                );
                Console.WriteLine(":)  Successfully read document (id, pk): (" + documentId + ", " + partitionKeyValue + ")");
                Console.WriteLine("... Trying to query for document");

               IQueryable <dynamic> query = client.CreateDocumentQuery("/dbs/" + databaseId + "/colls/" + collectionId, "SELECT * FROM c WHERE c.id = '" + documentId + "' AND c.partitionKey = '" + partitionKeyValue + "'");
                if(query.ToList().Count == 1)
                {
                    Console.WriteLine(":)  Found the document via query. Everything is looking good.");
                    return;
                }

                Console.WriteLine(":(  Could not find the document via query. Something is unbroken...");
                throw new Exception(":(  Could not find the document via query. Something is unbroken...");
            }
            catch (DocumentClientException dce)
            {
                if(secondTry)
                {
                    Console.WriteLine(":(  Tried to read after successful write and it didn't work. Something is wrong...");
                    throw new Exception(":(  Tried to read after successful write and it didn't work. Something is wrong...");
                }
                Console.WriteLine(":/  Could not read document, will try to write document, then try read again");
            }

            try
            {
                Console.WriteLine("... Trying to write document(id, pk): (" + documentId + ", " + partitionKeyValue + ")");

               ResourceResponse <Document> doc = await client.CreateDocumentAsync(
                    "/dbs/" + databaseId + "/colls/" + collectionId, 
                    new { id = documentId, partitionKey = partitionKeyValue }, 
                    new RequestOptions() { PartitionKey = new PartitionKey(partitionKeyValue) }
                );
                Console.WriteLine(":)  Successfully write document (id, pk): (" + documentId + ", " + partitionKeyValue + ")");
                await run();
                return;
            }
            catch (DocumentClientException dce)
            {
                Console.WriteLine(":(  Could not write document, will give up. Something is broken in the backend...");
                throw new Exception(":(  Could not write document, will give up. Something is broken in the backend...");
            }
        }
    }
}
