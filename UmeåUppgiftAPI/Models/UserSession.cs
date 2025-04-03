namespace UmeåUppgiftAPI.Models
{
    public class UserSession
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        public List<string> FavoriteCatIds { get; set; } = new List<string>();
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
        
        // Add any other user-specific properties you want to track
        public int PageVisits { get; set; } = 0;
        public string? PreferredCatBreed { get; set; }
    }
} 