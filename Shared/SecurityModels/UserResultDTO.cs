using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SecurityModels
{
    public record UserResultDTO
    {
        public UserResultDTO(string displayName, string email, string token)
        {
            DisplayName = displayName;
            Email = email;
            Token = token;
            Roles = new List<string>();
        }

        public UserResultDTO(string displayName, string email, string token, List<string> roles)
        {
            DisplayName = displayName;
            Email = email;
            Token = token;
            Roles = roles ?? new List<string>();
        }

        public string DisplayName { get; init; }
        public string Email { get; init; }
        public string Token { get; init; }
        public List<string> Roles { get; init; }
    }
}
