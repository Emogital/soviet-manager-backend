using System.ComponentModel.DataAnnotations;

namespace DataService.Models
{
    public class LobbySettings
    {
        [Key]
        [MaxLength(36)]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(16)]
        public string MatchName { get; set; } = string.Empty;

        [MaxLength(16)]
        public string PlayerName { get; set; } = string.Empty;

        [MaxLength(16)]
        public string PlayerNick { get; set; } = string.Empty;
    }
}
