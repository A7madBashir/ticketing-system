using System;

namespace TicketingSystem.Models.DTO.Responses.User
{
    public class UserResponseDto
    {
        public Ulid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public string Job { get; set; }
        public DateTime DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public Ulid AgencyId { get; set; }
        public string AgencyName { get; set; }
    }
}
