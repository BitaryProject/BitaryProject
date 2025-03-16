using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SecurityModels
{
    public record UserResultDTO(string DisplayName,string Email,string Token)
    {
    }
}
