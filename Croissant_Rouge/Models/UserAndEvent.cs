namespace Croissant_Rouge.Models
{
    public class UserAndEvent
    {
        public List<Event> Allevents { get; set; } = new();
        public User User { get; set; } = new User();
        public Participation Participation { get; set; } = new();
    }
}
