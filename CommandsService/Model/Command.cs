using System.ComponentModel.DataAnnotations;

namespace CommandsService.Model
{
    public class Command
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string HowTo { get; set; }
        [Required]
        public string CommandLine { get; set; }
        [Required]
        public int PlatformId { get; set; }

        // Navigation property
        public Platform Platform { get; set; }
    }
}