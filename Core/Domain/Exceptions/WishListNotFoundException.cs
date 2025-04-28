using System;

namespace Domain.Exceptions
{
    public class WishListNotFoundException : EntityNotFoundException
    {
        public WishListNotFoundException(string id)
            : base($"The wishlist with ID {id} was not found.")
        {
        }
    }
} 