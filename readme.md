# Cosmos Read Test

Tries to perform a read on a document, then an equivalent query. If it can't read the document, it tries to create that document and then retries the read/query. This sample is mostly used for troubleshooting.

## Dependencies

Azure Cosmos DB (emulator ok)
.NET Framework 4.6.1
Visual Studio (suggested)

## Build and run

Suggest using VS. Open the solution, and edit the content on line 29-34 with the appropriate values for your environment. This was tested against emulator. Then build and run the console app and look at the output.

## Contributing

This repo will mostly be ignored. Email me if you want some change (include the repo name in subject).

## LICENSE

[MIT](LICENSE)