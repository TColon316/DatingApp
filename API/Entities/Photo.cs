using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
  public int Id { get; set; }

  [Required]
  public required string Url { get; set; }
  public bool IsMain { get; set; }
  public string? PublicId { get; set; }

  // Navigation Properties - The below will create a REQUIRED one to many relationship between Photo and AppUser
  public int AppUserId { get; set; }
  public AppUser AppUser { get; set; } = null!;
}