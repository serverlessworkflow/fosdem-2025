namespace GalacticBountyNetwork.Controllers;

[Route("api/[controller]")]
public class BountiesController(IJsonSerializer jsonSerializer)
    : Controller
{

    static readonly List<Bounty> Bounties =
    [
        new Bounty
        {
            Id = "BNY-1001",
            TargetName = "Korr Vex",
            LastKnownLocation = "Mos Espa, Tatooine",
            Reward = 75000m,
            Status = "Open",
            IssuedBy = "Hutt Cartel",
            KnownAffiliations = new List<string> { "Hutt Cartel", "Zygerrian Slavers" },
            PostedDate = DateTime.UtcNow.AddDays(-10),
            ExpirationDate = DateTime.UtcNow.AddMonths(1),
            AliveOnly = true,
            AdditionalNotes = "Wanted alive for interrogation regarding spice smuggling operations."
        },
        new Bounty
        {
            Id = "BNY-1002",
            TargetName = "Liora Vex",
            LastKnownLocation = "Coronet City, Corellia",
            Reward = 120000m,
            Status = "Open",
            IssuedBy = "Imperial Intelligence",
            KnownAffiliations = new List<string> { "Black Sun" },
            PostedDate = DateTime.UtcNow.AddDays(-20),
            ExpirationDate = DateTime.UtcNow.AddMonths(2),
            AliveOnly = false,
            AdditionalNotes = "Former Imperial defector, now supplying Black Sun with stolen intel."
        },
        new Bounty
        {
            Id = "BNY-1003",
            TargetName = "Varis Skorn",
            LastKnownLocation = "Lower Levels, Nar Shaddaa",
            Reward = 50000m,
            Status = "In Progress",
            IssuedBy = "Galactic Banking Clan",
            KnownAffiliations = new List<string> { "Crimson Dawn" },
            PostedDate = DateTime.UtcNow.AddDays(-5),
            ExpirationDate = null,
            AliveOnly = false,
            AdditionalNotes = "Elimination contract. Has stolen encryption keys to several off-world accounts."
        },
        new Bounty
        {
            Id = "BNY-1004",
            TargetName = "Rok Daggath",
            LastKnownLocation = "Spaceport Theta, Ryloth",
            Reward = 95000m,
            Status = "Open",
            IssuedBy = "Trade Federation",
            KnownAffiliations = new List<string> { "Rodian Mercenary Guild" },
            PostedDate = DateTime.UtcNow.AddDays(-15),
            ExpirationDate = DateTime.UtcNow.AddMonths(3),
            AliveOnly = true,
            AdditionalNotes = "Captured Separatist war criminal, evaded Republic execution."
        },
        new Bounty
        {
            Id = "BNY-1005",
            TargetName = "Jarek Val",
            LastKnownLocation = "Uncharted Sectors, Outer Rim",
            Reward = 200000m,
            Status = "Open",
            IssuedBy = "Mandalorian Enclave",
            KnownAffiliations = new List<string> { "Death Watch" },
            PostedDate = DateTime.UtcNow.AddDays(-30),
            ExpirationDate = DateTime.UtcNow.AddMonths(4),
            AliveOnly = true,
            AdditionalNotes = "Ex-Mandalorian traitor selling beskar armor to off-worlders."
        },
        new Bounty
        {
            Id = "BNY-1006",
            TargetName = "Syra Vos",
            LastKnownLocation = "Dreshdae, Korriban",
            Reward = 175000m,
            Status = "Expired",
            IssuedBy = "Sith Intelligence",
            KnownAffiliations = new List<string> { "Lost Jedi Cells" },
            PostedDate = DateTime.UtcNow.AddDays(-90),
            ExpirationDate = DateTime.UtcNow.AddDays(-10),
            AliveOnly = false,
            AdditionalNotes = "Ex-Jedi Knight, fled Republic space after Order 66. Dead or alive."
        },
        new Bounty
        {
            Id = "BNY-1007",
            TargetName = "Krad Thorne",
            LastKnownLocation = "Junkfields, Raxus Prime",
            Reward = 45000m,
            Status = "Open",
            IssuedBy = "Corporate Sector Authority",
            KnownAffiliations = new List<string> { "Cybernetic Augmenters Syndicate" },
            PostedDate = DateTime.UtcNow.AddDays(-3),
            ExpirationDate = null,
            AliveOnly = false,
            AdditionalNotes = "Known black-market cybernetic enhancer. Extremely dangerous."
        },
        new Bounty
        {
            Id = "BNY-1008",
            TargetName = "Krix Vandros",
            LastKnownLocation = "Outlaw Station, Terminus",
            Reward = 60000m,
            Status = "Open",
            IssuedBy = "Imperial Bounty Registry",
            KnownAffiliations = new List<string> { "Rebel Alliance" },
            PostedDate = DateTime.UtcNow.AddDays(-25),
            ExpirationDate = DateTime.UtcNow.AddMonths(1),
            AliveOnly = true,
            AdditionalNotes = "Rebel pilot suspected of aiding the escape of key resistance leaders."
        },
        new Bounty
        {
            Id = "BNY-1009",
            TargetName = "Zyra Tal'Vex",
            LastKnownLocation = "Smuggler’s Haven, Mon Gazza",
            Reward = 110000m,
            Status = "Open",
            IssuedBy = "Hutt Cartel",
            KnownAffiliations = new List<string> { "Independent Smugglers Guild" },
            PostedDate = DateTime.UtcNow.AddDays(-7),
            ExpirationDate = DateTime.UtcNow.AddMonths(2),
            AliveOnly = false,
            AdditionalNotes = "Illegal arms dealer specializing in disruptor weapons. Execute if necessary."
        },
        new Bounty
        {
            Id = "BNY-1010",
            TargetName = "Garrik 'Ghost' Dorne",
            LastKnownLocation = "Unknown",
            Reward = 500000m,
            Status = "Open",
            IssuedBy = "Private Contractor",
            KnownAffiliations = new List<string> { "Red Mandalorians", "Elite Shadow Operatives" },
            PostedDate = DateTime.UtcNow.AddDays(-60),
            ExpirationDate = null,
            AliveOnly = true,
            AdditionalNotes = "Highly trained assassin. No known visual record. Extreme caution advised."
        }
    ];
    static readonly List<BountyTrackingUpdate> TrackingUpdates =
    [
        new BountyTrackingUpdate
        {
            Status = "Sighted",
            Location = "Mos Espa, Tatooine"
        },
        new BountyTrackingUpdate
        {
            Status = "Engaged",
            Location = "Coronet City, Corellia"
        },
        new BountyTrackingUpdate
        {
            Status = "Escaped",
            Location = "Lower Levels, Nar Shaddaa"
        },
        new BountyTrackingUpdate
        {
            Status = "Captured",
            Location = "Spaceport Theta, Ryloth"
        },
        new BountyTrackingUpdate
        {
            Status = "Unknown",
            Location = "Unknown Regions, Outer Rim"
        },
        new BountyTrackingUpdate
        {
            Status = "Sighted",
            Location = "Dreshdae, Korriban"
        },
        new BountyTrackingUpdate
        {
            Status = "Engaged",
            Location = "Junkfields, Raxus Prime"
        },
        new BountyTrackingUpdate
        {
            Status = "Sighted",
            Location = "Outlaw Station, Terminus"
        },
        new BountyTrackingUpdate
        {
            Status = "Escaped",
            Location = "Smuggler’s Haven, Mon Gazza"
        },
        new BountyTrackingUpdate
        {
            Status = "Sighted",
            Location = "Kamino"
        },
        new BountyTrackingUpdate
        {
            Status = "Engaged",
            Location = "Echo Base Ruins, Hoth"
        },
        new BountyTrackingUpdate
        {
            Status = "Captured",
            Location = "Shadowport 13, Wild Space"
        }
    ];

    [HttpGet]
    public ActionResult<List<Bounty>> ListBounties() => this.Ok(Bounties);

    [HttpPost("{bountyId}/contract/take")]
    public IActionResult TakeContract(string bountyId) => this.Accepted();

    [HttpPost("track")]
    public async Task<IActionResult> Track(CancellationToken cancellationToken = default)
    {
        this.Response.Headers.ContentType = "text/event-stream";
        this.Response.Headers.CacheControl = "no-cache";
        this.Response.Headers.Connection = "keep-alive";
        await this.Response.Body.FlushAsync();
        try
        {
            foreach (var e in TrackingUpdates)
            {
                var sseMessage = $"data: {jsonSerializer.SerializeToText(e)}\n\n";
                await this.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(sseMessage), cancellationToken).ConfigureAwait(false);
                await this.Response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
                await Task.Delay(Random.Shared.Next(250, 750));
            }
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException) { }
        return this.Ok();
    }

}