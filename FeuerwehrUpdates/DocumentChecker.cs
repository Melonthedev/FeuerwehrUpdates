﻿using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using FeuerwehrUpdates.Models;
using FeuerwehrUpdates.Services;
using Microsoft.EntityFrameworkCore;

namespace FeuerwehrUpdates
{
    public class DocumentChecker
    {


        private readonly HttpClient httpClient = new();
        private readonly PushService _pushService;
        private readonly ILogger<EinsatzListener> _logger;
        private readonly FWUpdatesDbContext _context;

        private readonly FUDocument _doc;
        private Einsatz? currentEinsatz;

        public DocumentChecker(FUDocument doc, PushService pushService, ILogger<EinsatzListener> logger, IDbContextFactory<FWUpdatesDbContext> contextFactory)
        {
            _doc = doc;
            _pushService = pushService;
            _logger = logger;
            _context = contextFactory.CreateDbContext();
        }

        public async Task Run()
        {
            var querySelector = _doc.QuerySelectorLatestOperation;
            _logger.LogInformation($"Getting latest Einsatz ({_doc.DocumentId}: {_doc.DocumentName})");
            var response = await httpClient.GetStringAsync(_doc.DocumentUrl);
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(response);
            var latestOperationEntry = document.QuerySelector(querySelector);

            Einsatz? einsatz = ParseEinsatzFromEntry(latestOperationEntry);
            if (einsatz is null) return;
            string tag;

            if (currentEinsatz == null)
            {
                currentEinsatz = einsatz;
                _logger.LogWarning($"Current einsatz not set! Setting current einsatz to latest: {currentEinsatz.EinsatzId} - {currentEinsatz.EinsatzInfo} ({_doc.DocumentName})");
                return;
            }

            if (currentEinsatz.EinsatzId != einsatz.EinsatzId)
            {
                tag = "neuer-einsatz";
                _context.Einsaetze.Add(einsatz);
                await _context.SaveChangesAsync();
            }
            //else if (currentEinsatz.PressLink != einsatz.PressLink) tag = "presslink-updated";
            else if (currentEinsatz.EinsatzInfo != einsatz.EinsatzInfo
                || currentEinsatz.Location != einsatz.Location
                || currentEinsatz.Vehicles != einsatz.Vehicles)
            {
                tag = "einsatz-updated";
                einsatz.Id = currentEinsatz.Id;
                _context.Einsaetze.Update(einsatz);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation($"No new entry detected. Latest: {currentEinsatz.EinsatzId} - {currentEinsatz.EinsatzInfo} ({_doc.DocumentName})");
                return;
            }

            _logger.LogInformation($"{_doc.DocumentName}: {einsatz.EinsatzId} - {einsatz.EinsatzInfo} - " +
                $"{einsatz.Location} - {einsatz.Vehicles} - {einsatz.EinsatzSchleifen} - " +
                $"{einsatz.Date} von {einsatz.StartedTime} bis {einsatz.EndTime}");

            var payload = new Payload()
            {
                Id = einsatz.Id,
                OperationId = einsatz.EinsatzId,
                Tag = tag,
                Title = "Einsatz: " + einsatz.EinsatzInfo,
                Content = $"{einsatz.EinsatzId}: {einsatz.StartedTime} - {einsatz.EndTime}\nOrt: {einsatz.Location}\nFahrzeuge: {einsatz.Vehicles}\nSchleife: {einsatz.EinsatzSchleifen}\n{einsatz.Date}",
                PressLink = einsatz.PressLink
            };

            await _pushService.SendPushNotificationToAll(payload);

            currentEinsatz = einsatz;
        }

        public Einsatz? ParseEinsatzFromEntry(IElement? tableEntry)
        {
            if (tableEntry == null)
            {
                _logger.LogError("Latest operationentry not found! (Invalid Query Selector?)");
                return null;
            }

            IElement presseLinkElement = null;
            if (_doc.PressLinkSelector != null)
            {
                presseLinkElement = tableEntry.QuerySelector(_doc.PressLinkSelector);
            }
            var presseLink = presseLinkElement == null ? null : presseLinkElement.Attributes.FirstOrDefault(attr => attr.Name == "href").Value;

            return new Einsatz()
            {
                DocumentName = _doc.DocumentName,
                DocumentUrl = _doc.DocumentUrl,
                EinsatzId = tableEntry.QuerySelector(_doc.IdSelector).TextContent.Trim().Replace(" ", string.Empty),
                Date = _doc.IdSelector is null ? null : tableEntry.QuerySelector(_doc.DateSelector).TextContent.Trim(),
                EinsatzInfo = _doc.InfoSelector is null ? null : tableEntry.QuerySelector(_doc.InfoSelector).TextContent.Trim(),
                EinsatzSchleifen = _doc.SchleifenSelector is null ? null : tableEntry.QuerySelector(_doc.SchleifenSelector).TextContent.Trim(),
                EndTime = _doc.EndTimeSelector is null ? null : tableEntry.QuerySelector(_doc.EndTimeSelector).TextContent.Trim(),
                StartedTime = _doc.StartTimeSelector is null ? null : tableEntry.QuerySelector(_doc.StartTimeSelector).TextContent.Trim(),
                Location = _doc.LocationSelector is null ? null : tableEntry.QuerySelector(_doc.LocationSelector).TextContent.Trim(),
                Vehicles = _doc.VehiclesSelector is null ? null : tableEntry.QuerySelector(_doc.VehiclesSelector).TextContent.Trim(),
                PressLink = presseLink
            };
        }
    }
}
