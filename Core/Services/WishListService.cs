using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ProductEntities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
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
        private readonly StoreContext _dbContext;

        public WishListService(IUnitOFWork unitOfWork, IMapper mapper, IProductService productService, StoreContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productService = productService;
            _dbContext = dbContext;
        }

        public async Task<WishListDTO> GetUserWishListAsync(string userId)
        {
            try
            {
                // Try to find the user's wishlist using DbContext
                var wishList = await _dbContext.WishLists
                    .Include(w => w.WishListItems)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.ProductBrand)
                    .Include(w => w.WishListItems)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.ProductCategory)
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                // If no wishlist exists, create a new one
                if (wishList == null)
                {
                    wishList = new WishList
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.WishLists.Add(wishList);
                    await _dbContext.SaveChangesAsync();
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
                var wishList = await _dbContext.WishLists
                    .Include(w => w.WishListItems)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.ProductBrand)
                    .Include(w => w.WishListItems)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.ProductCategory)
                    .FirstOrDefaultAsync(w => w.Id == id);

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

                // Get or create the user's wishlist from DbContext
                var wishList = await _dbContext.WishLists
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wishList == null)
                {
                    wishList = new WishList
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.WishLists.Add(wishList);
                    await _dbContext.SaveChangesAsync();
                }

                // Check if product already exists in wishlist
                var existingItem = await _dbContext.WishListItems
                    .FirstOrDefaultAsync(i => i.WishListId == wishList.Id && i.ProductId == productId);

                if (existingItem != null)
                {
                    // Load associated product details for mapping
                    await _dbContext.Entry(existingItem)
                        .Reference(i => i.Product)
                        .LoadAsync();

                    if (existingItem.Product != null)
                    {
                        await _dbContext.Entry(existingItem.Product)
                            .Reference(p => p.ProductBrand)
                            .LoadAsync();

                        await _dbContext.Entry(existingItem.Product)
                            .Reference(p => p.ProductCategory)
                            .LoadAsync();
                    }

                    return _mapper.Map<WishListItemDTO>(existingItem);
                }

                // Add new item to wishlist
                var wishListItem = new WishListItem
                {
                    WishListId = wishList.Id,
                    ProductId = productId,
                    AddedAt = DateTime.UtcNow
                };

                _dbContext.WishListItems.Add(wishListItem);
                await _dbContext.SaveChangesAsync();

                // Load associated entities for proper mapping
                await _dbContext.Entry(wishListItem)
                    .Reference(i => i.Product)
                    .LoadAsync();

                if (wishListItem.Product != null)
                {
                    await _dbContext.Entry(wishListItem.Product)
                        .Reference(p => p.ProductBrand)
                        .LoadAsync();

                    await _dbContext.Entry(wishListItem.Product)
                        .Reference(p => p.ProductCategory)
                        .LoadAsync();
                }

                return _mapper.Map<WishListItemDTO>(wishListItem);
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
                var item = await _dbContext.WishListItems.FindAsync(itemId);

                if (item == null)
                    throw new WishListItemNotFoundException(itemId.ToString());

                // Verify the item belongs to the user's wishlist
                if (item.WishListId != wishList.Id)
                    throw new InvalidOperationException("The wishlist item does not belong to this user's wishlist.");

                // Remove the item
                _dbContext.WishListItems.Remove(item);
                await _dbContext.SaveChangesAsync();

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

                // Use direct SQL or EF Core to remove items without tracking conflicts
                // Option 1: Use DbContext directly to avoid tracking issues
                var itemsToRemove = await _dbContext.WishListItems
                    .Where(item => item.WishListId == wishList.Id)
                    .ToListAsync();

                if (itemsToRemove.Any())
                {
                    _dbContext.WishListItems.RemoveRange(itemsToRemove);
                    await _dbContext.SaveChangesAsync();
                }

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
                // Check if the product exists in the user's wishlist using a direct query
                return await _dbContext.WishLists
                    .Where(w => w.UserId == userId)
                    .SelectMany(w => w.WishListItems)
                    .AnyAsync(i => i.ProductId == productId);
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
    }
} 