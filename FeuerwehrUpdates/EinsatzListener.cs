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

        public List<DocumentChecker> Checkers = new List<DocumentChecker>();

        public EinsatzListener(PushService pushService, IOptions<FUOptions> options, ILogger<EinsatzListener> logger)
        {
            _pushService = pushService;
            _options = options.Value;
            _logger = logger;
            Initialize();
        }

        public void Initialize()
        {
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

            foreach (FUDocument document in _options.Documents)
            {
                Checkers.Add(new DocumentChecker(document, _pushService, _logger));
            }

            Task CheckForChanges = PeriodicAsync(async () =>
            {
                try
                {
                    foreach (var checker in Checkers)
                        await checker.Run();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
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
