using System;

namespace TicketingSystem.Models.DTO.Requests.User
{
    public class UserRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public string Job { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Ulid AgencyId { get; set; }
    }
}
