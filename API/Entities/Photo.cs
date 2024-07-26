using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
  public int Id { get; set;}

  [Required]
  public required string URL { get; set; }

  public bool IsMain { get; set; }

  public string? PublicId { get; set; }

  // Navigation properties
  public int AppUserId { get; set; }

  // Use Null forgiving operator ("?")
  public AppUser AppUser { get; set; } = null!;
}