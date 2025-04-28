using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ProductEntities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Services.Abstractions;
using Services.Specifications;
using Shared.WishListModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class WishListService : IWishListService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public WishListService(IUnitOFWork unitOfWork, IMapper mapper, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productService = productService;
        }

        public async Task<WishListDTO> GetUserWishListAsync(string userId)
        {
            try
            {
                // Try to find the user's wishlist
                var spec = new WishListSpecification(userId);
                var wishList = await _unitOfWork.GetRepository<WishList, int>().GetAsync(spec);

                // If no wishlist exists, create a new one
                if (wishList == null)
                {
                    wishList = new WishList
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.GetRepository<WishList, int>().AddAsync(wishList);
                    await _unitOfWork.SaveChangesAsync();
                }

                return _mapper.Map<WishListDTO>(wishList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user wishlist: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<WishListDTO> GetWishListByIdAsync(int id)
        {
            try
            {
                var spec = new WishListSpecification(id);
                var wishList = await _unitOfWork.GetRepository<WishList, int>().GetAsync(spec);

                if (wishList == null)
                    throw new WishListNotFoundException(id.ToString());

                return _mapper.Map<WishListDTO>(wishList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting wishlist by ID: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<WishListItemDTO> AddItemToWishListAsync(string userId, int productId)
        {
            try
            {
                // Check if product exists
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                    throw new ProductNotFoundException(productId.ToString());

                // Get or create the user's wishlist
                var wishListSpec = new WishListSpecification(userId);
                var wishList = await _unitOfWork.GetRepository<WishList, int>().GetAsync(wishListSpec);

                if (wishList == null)
                {
                    wishList = new WishList
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.GetRepository<WishList, int>().AddAsync(wishList);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Check if product already exists in wishlist
                var wishListItemSpec = new WishListItemSpecification(wishList.Id, productId);
                var existingItem = await _unitOfWork.GetRepository<WishListItem, int>().GetAsync(wishListItemSpec);

                if (existingItem != null)
                    return _mapper.Map<WishListItemDTO>(existingItem);

                // Add new item to wishlist
                var wishListItem = new WishListItem
                {
                    WishListId = wishList.Id,
                    ProductId = productId,
                    AddedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<WishListItem, int>().AddAsync(wishListItem);
                await _unitOfWork.SaveChangesAsync();

                // Return the added item with product details
                var itemSpec = new WishListItemSpecification(wishListItem.Id);
                var itemWithDetails = await _unitOfWork.GetRepository<WishListItem, int>().GetAsync(itemSpec);

                return _mapper.Map<WishListItemDTO>(itemWithDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding item to wishlist: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<bool> RemoveItemFromWishListAsync(string userId, int itemId)
        {
            try
            {
                // Get the user's wishlist
                var wishListSpec = new WishListSpecification(userId);
                var wishList = await _unitOfWork.GetRepository<WishList, int>().GetAsync(wishListSpec);

                if (wishList == null)
                    throw new WishListNotFoundException(userId);

                // Get the wishlist item
                var item = await _unitOfWork.GetRepository<WishListItem, int>().GetAsync(itemId);

                if (item == null)
                    throw new WishListItemNotFoundException(itemId.ToString());

                // Verify the item belongs to the user's wishlist
                if (item.WishListId != wishList.Id)
                    throw new InvalidOperationException("The wishlist item does not belong to this user's wishlist.");

                // Remove the item
                _unitOfWork.GetRepository<WishListItem, int>().Delete(item);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item from wishlist: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<bool> ClearWishListAsync(string userId)
        {
            try
            {
                // Get the user's wishlist
                var wishListSpec = new WishListSpecification(userId);
                var wishList = await _unitOfWork.GetRepository<WishList, int>().GetAsync(wishListSpec);

                if (wishList == null)
                    return true; // Nothing to clear

                // Get all items in the wishlist
                var itemsSpec = new WishListItemSpecification(wishList.Id, true);
                var items = await _unitOfWork.GetRepository<WishListItem, int>().GetAllAsync(itemsSpec);

                // Remove each item
                foreach (var item in items)
                {
                    _unitOfWork.GetRepository<WishListItem, int>().Delete(item);
                }

                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing wishlist: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<bool> IsProductInWishListAsync(string userId, int productId)
        {
            try
            {
                // Get the user's wishlist
                var wishListSpec = new WishListSpecification(userId);
                var wishList = await _unitOfWork.GetRepository<WishList, int>().GetAsync(wishListSpec);

                if (wishList == null)
                    return false;

                // Check if product exists in wishlist
                var itemSpec = new WishListItemSpecification(wishList.Id, productId);
                var item = await _unitOfWork.GetRepository<WishListItem, int>().GetAsync(itemSpec);

                return item != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if product is in wishlist: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<bool> RemoveProductFromWishListAsync(string userId, int productId)
        {
            try
            {
                Console.WriteLine($"Removing product with ID {productId} from wishlist for user {userId}");
                
                // Get the user's wishlist
                var wishListSpec = new WishListSpecification(userId);
                var wishList = await _unitOfWork.GetRepository<WishList, int>().GetAsync(wishListSpec);

                if (wishList == null)
                {
                    Console.WriteLine($"Wishlist not found for user {userId}");
                    throw new WishListNotFoundException(userId);
                }

                Console.WriteLine($"Found wishlist with ID {wishList.Id}");

                // Find the wishlist item by productId
                var itemSpec = new WishListItemSpecification(wishList.Id, productId);
                var item = await _unitOfWork.GetRepository<WishListItem, int>().GetAsync(itemSpec);

                if (item == null)
                {
                    Console.WriteLine($"Product with ID {productId} not found in wishlist {wishList.Id}");
                    throw new Exception($"Product with ID {productId} not found in the wishlist");
                }

                Console.WriteLine($"Found wishlist item with ID {item.Id} for product {productId}");

                // Remove the item
                _unitOfWork.GetRepository<WishListItem, int>().Delete(item);
                await _unitOfWork.SaveChangesAsync();
                
                Console.WriteLine($"Successfully removed product {productId} from wishlist");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing product from wishlist: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
} 