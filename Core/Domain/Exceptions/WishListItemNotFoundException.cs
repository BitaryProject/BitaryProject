using System;

namespace Domain.Exceptions
{
    public class WishListItemNotFoundException : EntityNotFoundException
    {
        public WishListItemNotFoundException(string id)
            : base($"The wishlist item with ID {id} was not found.")
        {
        }
    }
} 