using System.ComponentModel.DataAnnotations;

namespace API.Helpers
{
    public class CloudinarySettings
    {
        [Required]
        public required string CloudName { get; set; }

        [Required]
        public required string ApiKey { get; set; }

        [Required]
        public required string ApiSecret { get; set; }
    }
}