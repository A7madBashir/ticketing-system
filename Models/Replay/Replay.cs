using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing-system.Models.Replay
{
    public class Replay
    {
        public int Id { get; set; } // Primary Key

        // Foreign Keys
        public int TicketId { get; set; }
        public int UserId { get; set; }

        public string Content { get; set; }
        public bool IsInternal { get; set; }
        public bool IsChatbotReply { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}