using System.ComponentModel.DataAnnotations;

namespace CommandsService.Model
{
    public class Platform
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int ExternalId { get; set; }
        [Required]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Command> Commands { get; set; } = new List<Command>();
    }
}