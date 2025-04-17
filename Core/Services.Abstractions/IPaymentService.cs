using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.BasketModels;

namespace Core.Services.Abstractions
{
    public interface IPaymentService
    {
        public Task<CustomerBasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId);
        public Task UpdateOrderPaymentStatus(string request, string stripeHeader);
        // msh ha3ml return 3shan stripe msh 3ayza meni response heya bas hat2oly eh ely hasal 


    }
}

