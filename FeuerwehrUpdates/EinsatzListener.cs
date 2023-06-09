using AngleSharp.Html.Parser;
using FeuerwehrUpdates.Models;
using FeuerwehrUpdates.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace FeuerwehrUpdates
{
    public class EinsatzListener
    {

        private readonly HttpClient httpClient = new();
        private readonly PushService _pushService;
        private readonly FUOptions _options;

        public string? currentOperationId;

        public EinsatzListener(PushService pushService, IOptions<FUOptions> options)
        {
            _pushService = pushService;
            _options = options.Value;
            Initialize();
        }

        public void Initialize()
        {
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            Task CheckForChanges = PeriodicAsync(async () =>
            {
                try
                {
                    var querySelector = _options.QuerySelectorLatestOperation;
                    var response = await httpClient.GetStringAsync(_options.DocumentUrl);
                    await Console.Out.WriteLineAsync($"Getting latest EINSATZinformationen with querySelector: ({querySelector})");
                    await Console.Out.WriteLineAsync("Fetching HTML...");

                    await Console.Out.WriteLineAsync("Parsing HTML...");
                    var parser = new HtmlParser();
                    var document = await parser.ParseDocumentAsync(response);
                    var latestOperationEntry = document.QuerySelector(querySelector);

                    if (latestOperationEntry == null)
                    {
                        await Console.Out.WriteLineAsync("Latest operationentry not found!");
                        return;
                    }

                    var einsatzId = latestOperationEntry.QuerySelector("td:nth-child(2)").TextContent.Trim().Replace(" ", string.Empty);
                    var einsatzDatum = latestOperationEntry.QuerySelector("td:nth-child(3)").TextContent.Trim();
                    var einsatzStart = latestOperationEntry.QuerySelector("td:nth-child(4)").TextContent.Trim();
                    var einsatzStichwort = latestOperationEntry.QuerySelector("td:nth-child(5)").TextContent.Trim();
                    var einsatzOrt = latestOperationEntry.QuerySelector("td:nth-child(6)").TextContent.Trim();
                    var einsatzFahrzeuge = latestOperationEntry.QuerySelector("td:nth-child(7)").TextContent.Trim();
                    var einsatzEnde = latestOperationEntry.QuerySelector("td:nth-child(8)").TextContent.Trim();
                    var schleife = latestOperationEntry.QuerySelector("td:nth-child(9)").TextContent.Trim();
                    var presseLinkElement = latestOperationEntry.QuerySelector("td:nth-child(10) a");
                    var presseLink = presseLinkElement == null ? null : presseLinkElement.Attributes.FirstOrDefault(attr => attr.Name == "href").Value;

                    await Console.Out.WriteLineAsync("Einsatz ID: " + einsatzId + "\nEinsatz Datum: " + einsatzDatum + "\nALARRRRMMM: " + einsatzStart 
                        + "\nWat los?: " + einsatzStichwort + "\nEinsatz Ort: " + einsatzOrt + "\nFahrzeuge: " + einsatzFahrzeuge 
                        + "\nEinsatz Ende: " + einsatzEnde + "\nSchleife: " + schleife + "\nPresseartikel: " + presseLink);

                    if (currentOperationId == null)
                    {
                        currentOperationId = einsatzId;
                        await Console.Out.WriteLineAsync("Setting current operation id.");
                        return;
                    }

                    if (currentOperationId == einsatzId)
                    {
                        await Console.Out.WriteLineAsync("No new entry detected.");
                        return;
                    }

                    var payload = new Payload()
                    {
                        Id = einsatzId,
                        Tag = "neuer-einsatz",
                        Title = "Einsatz: " + einsatzStichwort,
                        Content = $"{einsatzId}: {einsatzStart} - {einsatzEnde}\nOrt: {einsatzOrt}\nFahrzeuge: {einsatzFahrzeuge}\nSchleife: {schleife}\n{einsatzDatum}",
                        PressLink = presseLink,
                    };
                    await _pushService.SendPushNotificationToAll(payload);
                    currentOperationId = einsatzId;
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }
            }, TimeSpan.FromMinutes(_options.CheckForChangesIntervalInMinutes));
        }


        public async Task PeriodicAsync(Func<Task> action, TimeSpan interval, 
            CancellationToken cancellationToken = default)
        {
            using var timer = new PeriodicTimer(interval);
            while (true)
            {
                await action();
                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }
    }
}
