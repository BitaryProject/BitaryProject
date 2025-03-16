using Shared.OrderModels;
using Shared.SecurityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IAuthenticationService
    {
        public Task<UserResultDTO> LoginAsync(LoginDTO loginModel);
        public Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel);

        public Task<UserResultDTO> GetUserByEmail(string email);

        public Task<bool> CheckEmailExist(string email);

        public Task<AddressDTO> GetUserAddress(string email);

        public Task<AddressDTO> UpdateUserAddress(AddressDTO address,string email);


    }
}
