using System;
namespace Croissant_Rouge.Models
{
    public class DonationandUser
    {
        public List<Donation> Alldonations { get; set; } = new();
        public User User { get; set; } = new User();
    }
}
