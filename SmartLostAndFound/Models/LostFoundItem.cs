using System.ComponentModel.DataAnnotations;

namespace SmartLostAndFound.Models;

public class LostFoundItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public bool IsClaimed { get; set; }
    public string ImagePath { get; set; } = string.Empty;

    [Required]
    public string FinderName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string FinderEmail { get; set; } = string.Empty;

    [Required]
    public string PickupLocation { get; set; } = string.Empty;

    public string PickupInstructions { get; set; } = string.Empty;
}