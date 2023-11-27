using Croissant_Rouge.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace Croissant_Rouge.Models
{
    public class Participation
    {
        [Key]
        public int ParticipationId { get; set; }

        public int UserId { get; set; }
        public User? Participator { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        // Created At
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Updated At
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Add the navigation property Here
    }
}
