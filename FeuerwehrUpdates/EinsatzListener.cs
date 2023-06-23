using AngleSharp.Dom;
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
        private readonly ILogger<EinsatzListener> _logger;

        public string? currentOperationId;
        public Einsatz? currentEinsatz;

        public EinsatzListener(PushService pushService, IOptions<FUOptions> options, ILogger<EinsatzListener> logger)
        {
            _pushService = pushService;
            _options = options.Value;
            _logger = logger;
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
                    _logger.LogInformation($"Getting latest EINSATZinformationen with querySelector: ({querySelector})");
                    _logger.LogInformation("Fetching HTML...");

                    _logger.LogInformation("Parsing HTML...");
                    var parser = new HtmlParser();
                    var document = await parser.ParseDocumentAsync(response);
                    var latestOperationEntry = document.QuerySelector(querySelector);

                    Einsatz? einsatz = await ParseEinsatzFromEntry(latestOperationEntry);
                    if (einsatz is null) return;
                    string tag = "neuer-einsatz";

                    if (currentEinsatz == null)
                    {
                        currentEinsatz = einsatz;
                        _logger.LogWarning("Current einsatz not set! Setting current einsatz.");
                        return;
                    }

                    if (currentEinsatz.Id == einsatz.Id)
                    {
                        _logger.LogInformation("No new entry detected.");
                        return;
                    }

                    _logger.LogInformation("NEW EINSATZ!\nID: " + einsatz.Id + "\nDatum: " + einsatz.Date + "\nALARRRRMMM: " + einsatz.StartedTime
                        + "\nStichwort: " + einsatz.EinsatzInfo + "\nOrt: " + einsatz.Location + "\nFahrzeuge: " + einsatz.Vehicles
                        + "\nEnde: " + einsatz.EndTime + "\nSchleife: " + einsatz.EinsatzSchleifen + "\nPresseartikel: " + einsatz.PressLink);

                    var payload = new Payload()
                    {
                        Id = einsatz.Id,
                        Tag = "neuer-einsatz",
                        Title = "Einsatz: " + einsatz.EinsatzInfo,
                        Content = $"{einsatz.Id}: {einsatz.StartedTime} - {einsatz.EndTime}\nOrt: {einsatz.Location}\nFahrzeuge: {einsatz.Vehicles}\nSchleife: {einsatz.EinsatzSchleifen}\n{einsatz.Date}",
                        PressLink = einsatz.PressLink,
                    };
                    await _pushService.SendPushNotificationToAll(payload);
                    currentEinsatz = einsatz;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
            }, TimeSpan.FromMinutes(_options.CheckForChangesIntervalInMinutes));
        }

        public async Task<Einsatz?> ParseEinsatzFromEntry(IElement? tableEntry)
        {
            if (tableEntry == null)
            {
                _logger.LogError("Latest operationentry not found! (Invalid HTML doc?)");
                return null;
            }
            var einsatzId = tableEntry.QuerySelector("td:nth-child(2)").TextContent.Trim().Replace(" ", string.Empty);
            var einsatzDatum = tableEntry.QuerySelector("td:nth-child(3)").TextContent.Trim();
            var einsatzStart = tableEntry.QuerySelector("td:nth-child(4)").TextContent.Trim();
            var einsatzStichwort = tableEntry.QuerySelector("td:nth-child(5)").TextContent.Trim();
            var einsatzOrt = tableEntry.QuerySelector("td:nth-child(6)").TextContent.Trim();
            var einsatzFahrzeuge = tableEntry.QuerySelector("td:nth-child(7)").TextContent.Trim();
            var einsatzEnde = tableEntry.QuerySelector("td:nth-child(8)").TextContent.Trim();
            var schleife = tableEntry.QuerySelector("td:nth-child(9)").TextContent.Trim();
            var presseLinkElement = tableEntry.QuerySelector("td:nth-child(10) a");
            var presseLink = presseLinkElement == null ? null : presseLinkElement.Attributes.FirstOrDefault(attr => attr.Name == "href").Value;
            return new Einsatz()
            {
                Id = einsatzId,
                Date = einsatzDatum,
                EinsatzInfo = einsatzStichwort,
                EinsatzSchleifen = schleife,
                EndTime = einsatzEnde,
                StartedTime = einsatzStart,
                Location = einsatzOrt,
                Vehicles = einsatzFahrzeuge,
                PressLink = presseLink
            };
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
