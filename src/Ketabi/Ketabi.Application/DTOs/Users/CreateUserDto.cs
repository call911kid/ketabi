using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ketabi.Application.DTOs.Users
{
    public class CreateUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public double ReputationScore { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
