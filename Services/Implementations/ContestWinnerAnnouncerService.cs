using TalentShowcase.Api.Models.Constants;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    // Polls for contests whose EndDate has passed and haven't had a winner announced yet,
    // crowns the top-voted entry (only if it actually has at least 1 vote), then emails +
    // notifies the winner. Runs in its own DI scope per tick since Contest/Notification/Email
    // are Scoped services, not Singleton.
    public class ContestWinnerAnnouncerService : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ContestWinnerAnnouncerService> _logger;

        public ContestWinnerAnnouncerService(IServiceScopeFactory scopeFactory, ILogger<ContestWinnerAnnouncerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await AnnounceWinnersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Contest winner announcement pass failed.");
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
        }

        private async Task AnnounceWinnersAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var contestRepo = scope.ServiceProvider.GetRequiredService<IContestRepository>();
            var entryRepo = scope.ServiceProvider.GetRequiredService<IContestEntryRepository>();
            var voteRepo = scope.ServiceProvider.GetRequiredService<IContestVoteRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var endedContests = await contestRepo.GetEndedUnprocessedAsync();

            foreach (var contest in endedContests)
            {
                stoppingToken.ThrowIfCancellationRequested();

                var topEntry = (await entryRepo.GetByContestIdAsync(contest.Id, 1, 1)).FirstOrDefault();

                // Only crown a winner if someone actually got at least 1 vote — an entry with
                // 0 votes just happens to sort first, it didn't "win" anything.
                if (topEntry != null && await voteRepo.CountByEntryIdAsync(topEntry.Id) > 0)
                {
                    contest.WinnerEntryId = topEntry.Id;

                    var winner = topEntry.Video.User;
                    await notificationService.CreateAsync(
                        winner.Id,
                        $"Congratulations! Your entry won the contest \"{contest.Title}\".",
                        ReferenceTypes.Contest,
                        contest.Id);

                    try
                    {
                        await emailService.SendContestWinnerAsync(winner.Email, winner.Username, contest.Title);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send contest winner email for contest {ContestId}.", contest.Id);
                    }
                }

                contest.WinnerAnnouncedAt = DateTime.UtcNow;
                contestRepo.Update(contest);
                await contestRepo.SaveChangesAsync();
            }
        }
    }
}
