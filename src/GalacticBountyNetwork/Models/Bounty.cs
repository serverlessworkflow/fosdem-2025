namespace GalacticBountyNetwork.Models;

public class Bounty
{

    public required string Id { get; set; }

    public required string TargetName { get; set; }

    public required string LastKnownLocation { get; set; }

    public decimal Reward { get; set; }

    public required string Status { get; set; }

    public required string IssuedBy { get; set; }

    public List<string>? KnownAffiliations { get; set; }

    public DateTime PostedDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool AliveOnly { get; set; }

    public string? AdditionalNotes { get; set; }

}
