using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Domain.Entities.OrderEntities;
using Services.Specifications;
using OrderAddress = Domain.Entities.OrderEntities.Address;

using Shared.BasketModels;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;
using Address = Domain.Entities.OrderEntities.Address;



namespace Services
{
    public class OrderService(IUnitOFWork unitOfWork ,
        IMapper mapper,
        IbasketRepository basketRepository)
        : IOrderService
    {
        public async Task<OrderResult?> CreateOrUpdateOrderAsync(OrderRequest request, string userEmail)
        {
            try
            {
                Console.WriteLine($"Starting order creation. Email: {userEmail}, BasketId: {request.BasketId}");
                
                // Validate email is not null or empty
                if (string.IsNullOrEmpty(userEmail))
                {
                    Console.WriteLine("Warning: userEmail is null or empty. Using default guest email.");
                    userEmail = "guest@example.com";
                }
                
                Console.WriteLine("Mapping shipping address");
                var address = mapper.Map<Address>(request.ShippingAddress);
                Console.WriteLine($"Mapped address: Name={address.Name}, Street={address.Street}, City={address.City}, Country={address.Country}");

                Console.WriteLine($"Getting basket with ID: {request.BasketId}");
                var basket = await basketRepository.GetBasketAsync(Guid.Parse(request.BasketId))
                    ?? throw new BasketNotFoundException(request.BasketId);
                
                Console.WriteLine($"Found basket with {basket.BasketItems?.Count() ?? 0} items and PaymentIntentId: {basket.PaymentIntentId}");

                // Check for valid PaymentIntentId
                if (string.IsNullOrEmpty(basket.PaymentIntentId))
                {
                    Console.WriteLine("Warning: PaymentIntentId is null or empty. Setting default value.");
                    basket.PaymentIntentId = "default_" + Guid.NewGuid().ToString();
                    await basketRepository.UpdateBasketAsync(basket);
                }

                var orderItems = new List<OrderItem>();
                Console.WriteLine("Creating order items");
                foreach (var item in basket.BasketItems)
                {
                    Console.WriteLine($"Processing basket item: ProductId={item.Product?.ProductId}, Quantity={item.Quantity}");
                    
                    // Make sure the basket item has a valid product
                    if (item.Product == null)
                    {
                        Console.WriteLine("WARNING: Basket item has null Product. Skipping this item.");
                        continue;
                    }
                    
                    var product = await unitOfWork.GetRepository<Product, int>()
                        .GetAsync(item.Product.ProductId) ?? throw new ProductNotFoundException(item.Product.ProductId);
                    
                    Console.WriteLine($"Found product: Id={product.Id}, Name={product.Name}, Price={product.Price}");
                    orderItems.Add(CreateOrderItem(item, product));
                }
                Console.WriteLine($"Created {orderItems.Count} order items");

                // Get delivery method and set only the ID reference to avoid EF trying to insert a new record
                Console.WriteLine($"Getting delivery method: {request.DeliveryMethodId}");
                var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>()
                    .GetAsync(request.DeliveryMethodId)
                    ?? throw new DeliveryMethodNotFoundException(request.DeliveryMethodId);
                
                Console.WriteLine($"Found delivery method: {deliveryMethod.ShortName}, Price: {deliveryMethod.Price}");
                
                // Store these for later use
                var deliveryMethodId = deliveryMethod.Id;
                var deliveryMethodPrice = deliveryMethod.Price;

                var orderRepo = unitOfWork.GetRepository<Order, Guid>();

                Console.WriteLine($"Checking for existing order with PaymentIntentId: {basket.PaymentIntentId}");
                var existingOrder = await orderRepo.
                    GetAsync(new OrderWithPaymentIntentIdSpecifications(basket.PaymentIntentId));

                if (existingOrder is not null)
                {
                    Console.WriteLine($"Found existing order with ID: {existingOrder.Id}. Deleting it.");
                    orderRepo.Delete(existingOrder);
                }

                // subtotal
                var subTotal = orderItems.Sum(item => item.Price * item.Quantity);
                Console.WriteLine($"Calculated subtotal: {subTotal}");

                // save to DB
                Console.WriteLine("Creating new order");
                
                // Ensure PaymentIntentId is never null or empty when creating the order
                var paymentIntentId = !string.IsNullOrEmpty(basket.PaymentIntentId) 
                    ? basket.PaymentIntentId 
                    : $"default_{Guid.NewGuid()}";
                
                // Create order using the ID reference instead of the full entity
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserEmail = userEmail,
                    ShippingAddress = address,
                    OrderItems = orderItems.ToList(),
                    DeliveryMethodId = deliveryMethodId, // Use ID instead of the entity
                    Subtotal = subTotal,
                    PaymentIntentId = paymentIntentId,
                    OrderDate = DateTimeOffset.Now,
                    PaymentStatus = OrderPaymentStatus.Pending
                };
                
                // Log each order item 
                Console.WriteLine($"Order created with {orderItems.Count} items");
                foreach (var item in orderItems)
                {
                    Console.WriteLine($"Order item: ProductId={item.Product?.ProductId}, Quantity={item.Quantity}, Price={item.Price}");
                }
                
                Console.WriteLine($"Created order with ID: {order.Id}");
                
                Console.WriteLine("Adding order to repository");
                await unitOfWork.GetRepository<Order, Guid>()
                    .AddAsync(order);

                // Debug output of full order object
                Console.WriteLine("FULL ORDER DETAILS BEFORE SAVE:");
                Console.WriteLine($"Order ID: {order.Id}");
                Console.WriteLine($"User Email: {order.UserEmail}");
                Console.WriteLine($"Payment Intent ID: {order.PaymentIntentId}");
                Console.WriteLine($"Delivery Method ID: {order.DeliveryMethodId}");
                Console.WriteLine($"Subtotal: {order.Subtotal}");
                Console.WriteLine($"Address - Name: {order.ShippingAddress?.Name ?? "NULL"}, " +
                                  $"Street: {order.ShippingAddress?.Street ?? "NULL"}, " +
                                  $"City: {order.ShippingAddress?.City ?? "NULL"}, " +
                                  $"Country: {order.ShippingAddress?.Country ?? "NULL"}");
                Console.WriteLine($"Order items count: {order.OrderItems?.Count ?? 0}");
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        Console.WriteLine($"Item Product ID: {item.Product?.ProductId ?? 0}, " +
                                          $"Name: {item.Product?.ProductName ?? "NULL"}, " +
                                          $"Quantity: {item.Quantity}, " +
                                          $"Price: {item.Price}");
                    }
                }

                Console.WriteLine("Saving changes to the database");
                try
                {
                    // Simple save without transaction since transaction methods are not available in IUnitOfWork
                    await unitOfWork.SaveChangesAsync();
                    Console.WriteLine("Successfully saved order to the database");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"DATABASE UPDATE ERROR: {dbEx.Message}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"INNER EXCEPTION: {dbEx.InnerException.Message}");
                        Console.WriteLine($"STACK TRACE: {dbEx.InnerException.StackTrace}");
                    }
                    
                    // Common database errors
                    var errorMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                    if (errorMsg.Contains("FOREIGN KEY") || errorMsg.Contains("FK_"))
                    {
                        Console.WriteLine("LIKELY CAUSE: Foreign key constraint violation - Check relationships");
                    }
                    else if (errorMsg.Contains("UNIQUE") || errorMsg.Contains("Duplicate") || errorMsg.Contains("IX_"))
                    {
                        Console.WriteLine("LIKELY CAUSE: Unique constraint violation - Check for duplicate values");
                    }
                    else if (errorMsg.Contains("null") || errorMsg.Contains("NOT NULL"))
                    {
                        Console.WriteLine("LIKELY CAUSE: Required field missing - Check for null values");
                    }
                    
                    throw new Exception($"Order creation failed: {errorMsg}", dbEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR SAVING ORDER: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"INNER EXCEPTION: {ex.InnerException.Message}");
                        Console.WriteLine($"STACK TRACE: {ex.InnerException.StackTrace}");
                    }
                    throw new Exception("Order creation failed. See server logs for details.", ex);
                }

                // Map and Return
                Console.WriteLine("Mapping order to OrderResult");
                var result = mapper.Map<OrderResult>(order);
                
                // Set the total manually to ensure it includes delivery price
                result.Total = order.Subtotal + deliveryMethodPrice;
                
                Console.WriteLine($"Returning OrderResult with ID: {result.Id}, Total: {result.Total}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateOrUpdateOrderAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Rethrow to be handled by the controller
            }
        }

        private OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            // Validate inputs
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            
            // Create product item with null checks
            var productItem = new ProductInOrderItem(
                product.Id,
                product.Name ?? "Unknown Product",
                product.PictureUrl ?? "");
                
            return new OrderItem(
                productItem,
                Math.Max(1, item.Quantity), // Ensure quantity is at least 1
                product.Price);
        }


        public async Task<IEnumerable<DeliveryMethodResult?>> GetDeliveryMethodResult()
        {
            var methods = await unitOfWork.GetRepository<DeliveryMethod, int>()
                 .GetAllAsync();

            return mapper.Map<IEnumerable<DeliveryMethodResult>>(methods);
        }

        public async Task<OrderResult> GetOrderByIdAsync(Guid id)
        {
            try
            {
                Console.WriteLine($"OrderService.GetOrderByIdAsync - Retrieving order with ID: {id}");
                
                Console.WriteLine("Creating specification with OrderItems.Product include");
                var specification = new OrderWithIncludeSpecifications(id);
                
                Console.WriteLine("Getting order from repository using specification");
                var order = await unitOfWork.GetRepository<Order, Guid>()
                    .GetAsync(specification)
                    ?? throw new OrderNotFoundException(id);
                
                Console.WriteLine($"Retrieved order {order.Id} with {order.OrderItems?.Count ?? 0} items, Status: {order.PaymentStatus}");
                
                // Log order items for diagnostic purposes
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems.Take(5))
                    {
                        Console.WriteLine($"  - Item: ProductId={item.Product?.ProductId}, Name={item.Product?.ProductName}, Qty={item.Quantity}");
                    }
                }
                
                Console.WriteLine("Mapping order to OrderResult");
                var result = mapper.Map<OrderResult>(order);
                
                // Verify mapping result
                Console.WriteLine($"Mapped Order {result.Id}: {result.OrderItems?.Count() ?? 0} items, Status: {result.PaymentStatus}");
                
                // Check if we need to manually map order items
                if ((result.OrderItems == null || !result.OrderItems.Any()) && 
                    order.OrderItems != null && 
                    order.OrderItems.Any())
                {
                    Console.WriteLine("*** APPLYING MANUAL ORDER ITEMS MAPPING ***");
                    
                    // Clear any existing items and manually map them
                    var orderItems = new List<OrderItemDTO>();
                    
                    foreach (var item in order.OrderItems)
                    {
                        if (item.Product != null)
                        {
                            orderItems.Add(new OrderItemDTO
                            {
                                ProductId = item.Product.ProductId,
                                ProductName = item.Product.ProductName,
                                PictureUrl = item.Product.PictureUrl,
                                Quantity = item.Quantity,
                                Price = item.Price
                            });
                        }
                    }
                    
                    // Replace the empty collection with our manually mapped items
                    result.OrderItems = orderItems;
                    Console.WriteLine($"Manually mapped {result.OrderItems.Count()} items for order {result.Id}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetOrderByIdAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<IEnumerable<OrderResult>> GetOrderByEmailAsync(string email)
        {
            Console.WriteLine($"OrderService.GetOrderByEmailAsync - Retrieving orders for email: {email}");
            
            try 
            {
                Console.WriteLine("Creating specification with OrderItems.Product include");
                var specification = new OrderWithIncludeSpecifications(email);
                
                Console.WriteLine("Getting orders from repository using specification");
                var orders = await unitOfWork.GetRepository<Order, Guid>()
                    .GetAllAsync(specification);
                
                Console.WriteLine($"Retrieved {orders.Count()} orders");
                
                // If no orders found and this is not a guest user, try getting orders for guest user
                if (!orders.Any() && !string.Equals(email, "guest@example.com", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"No orders found for {email}, checking if any orders exist in the system");
                    
                    // Check if any orders exist in the system
                    var allOrders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync();
                    if (!allOrders.Any())
                    {
                        Console.WriteLine("No orders exist in the system at all");
                    }
                    else
                    {
                        Console.WriteLine($"Found {allOrders.Count()} orders in the system, but none for {email}");
                        
                        // Log the emails that have orders
                        var emailsWithOrders = allOrders.Select(o => o.UserEmail).Distinct().ToList();
                        Console.WriteLine($"Emails with orders: {string.Join(", ", emailsWithOrders)}");
                    }
                }
                
                // Log the first few orders for diagnostic purposes
                foreach (var order in orders.Take(2))
                {
                    Console.WriteLine($"Order {order.Id}: {order.OrderItems?.Count ?? 0} items, Status: {order.PaymentStatus}");
                    
                    if (order.OrderItems != null)
                    {
                        foreach (var item in order.OrderItems.Take(3))
                        {
                            Console.WriteLine($"  - Item: ProductId={item.Product?.ProductId}, Name={item.Product?.ProductName}, Qty={item.Quantity}");
                        }
                    }
                }
                
                Console.WriteLine("Mapping orders to OrderResult");
                var result = mapper.Map<IEnumerable<OrderResult>>(orders);
                
                // Verify mapping results
                var resultList = result.ToList();
                Console.WriteLine($"Mapped {resultList.Count} orders with order items:");
                foreach (var mappedOrder in resultList.Take(2))
                {
                    Console.WriteLine($"Mapped Order {mappedOrder.Id}: {mappedOrder.OrderItems?.Count() ?? 0} items, Status: {mappedOrder.PaymentStatus}");
                }
                
                // Check if we need to manually map order items
                bool needsManualMapping = false;
                foreach (var mappedOrder in resultList)
                {
                    if ((mappedOrder.OrderItems == null || !mappedOrder.OrderItems.Any()) && 
                        orders.FirstOrDefault(o => o.Id == mappedOrder.Id)?.OrderItems?.Any() == true)
                    {
                        needsManualMapping = true;
                        Console.WriteLine($"Order {mappedOrder.Id} has items in the entity but not in the DTO - manual mapping needed");
                        break;
                    }
                }
                
                // If AutoMapper failed to map OrderItems correctly, do it manually
                if (needsManualMapping)
                {
                    Console.WriteLine("*** APPLYING MANUAL ORDER ITEMS MAPPING ***");
                    var ordersDict = orders.ToDictionary(o => o.Id, o => o);
                    
                    foreach (var mappedOrder in resultList)
                    {
                        if (ordersDict.TryGetValue(mappedOrder.Id, out var originalOrder) && 
                            originalOrder.OrderItems != null && 
                            originalOrder.OrderItems.Any())
                        {
                            // Clear any existing items and manually map them
                            var orderItems = new List<OrderItemDTO>();
                            
                            foreach (var item in originalOrder.OrderItems)
                            {
                                if (item.Product != null)
                                {
                                    orderItems.Add(new OrderItemDTO
                                    {
                                        ProductId = item.Product.ProductId,
                                        ProductName = item.Product.ProductName,
                                        PictureUrl = item.Product.PictureUrl,
                                        Quantity = item.Quantity,
                                        Price = item.Price
                                    });
                                }
                            }
                            
                            // Replace the empty collection with our manually mapped items
                            mappedOrder.OrderItems = orderItems;
                            Console.WriteLine($"Manually mapped {mappedOrder.OrderItems.Count()} items for order {mappedOrder.Id}");
                        }
                    }
                }
                
                return resultList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetOrderByEmailAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        //GetAllOrders with filteration in db
        public async Task<IEnumerable<OrderResult>> GetAllOrdersAsync(string? status = null, int pageNumber = 1, int pageSize = 10)
        {
            Console.WriteLine($"OrderService.GetAllOrdersAsync - Starting with status: {status ?? "None"}, page: {pageNumber}, size: {pageSize}");
            
            try
            {
                var orderRepo = unitOfWork.GetRepository<Order, Guid>();
                
                Console.WriteLine("OrderService - Got repository instance");
    
                var ordersQuery = orderRepo.GetAllAsQueryable();
                
                Console.WriteLine("OrderService - Got queryable. Checking if it's null...");
                
                if (ordersQuery == null)
                {
                    Console.WriteLine("WARNING: GetAllAsQueryable returned null. Using empty list as fallback.");
                    return new List<OrderResult>();
                }
                
                // Count total orders in the system for diagnostic purposes
                var totalOrderCount = ordersQuery.Count();
                Console.WriteLine($"OrderService - Total orders in database: {totalOrderCount}");
    
                if (!string.IsNullOrEmpty(status))
                {
                    Console.WriteLine($"OrderService - Applying status filter: {status}");
                    ordersQuery = ordersQuery.Where(o => o.PaymentStatus.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
                }
    
                var paginatedOrders = ordersQuery
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
                    
                Console.WriteLine("OrderService - Applied pagination");
    
                var orders = await paginatedOrders.ToListAsync();
                
                Console.WriteLine($"OrderService - Retrieved {orders.Count} orders after pagination");
                
                if (orders.Count == 0)
                {
                    Console.WriteLine("OrderService - No orders found after applying filters and pagination");
                }
                else
                {
                    // Log some details about the retrieved orders
                    foreach (var order in orders.Take(3)) // Log just first 3 for brevity
                    {
                        Console.WriteLine($"Order Sample - ID: {order.Id}, Email: {order.UserEmail}, Status: {order.PaymentStatus}, Date: {order.OrderDate}");
                    }
                }
    
                var mappedResults = mapper.Map<IEnumerable<OrderResult>>(orders);
                Console.WriteLine($"OrderService - Successfully mapped {mappedResults.Count()} orders to OrderResult");
                
                // Verify mapping results and handle manual mapping if needed
                var resultList = mappedResults.ToList();
                bool needsManualMapping = false;
                
                foreach (var mappedOrder in resultList)
                {
                    if ((mappedOrder.OrderItems == null || !mappedOrder.OrderItems.Any()) && 
                        orders.FirstOrDefault(o => o.Id == mappedOrder.Id)?.OrderItems?.Any() == true)
                    {
                        needsManualMapping = true;
                        Console.WriteLine($"Order {mappedOrder.Id} has items in the entity but not in the DTO - manual mapping needed");
                        break;
                    }
                }
                
                // If AutoMapper failed to map OrderItems correctly, do it manually
                if (needsManualMapping)
                {
                    Console.WriteLine("*** APPLYING MANUAL ORDER ITEMS MAPPING ***");
                    var ordersDict = orders.ToDictionary(o => o.Id, o => o);
                    
                    foreach (var mappedOrder in resultList)
                    {
                        if (ordersDict.TryGetValue(mappedOrder.Id, out var originalOrder) && 
                            originalOrder.OrderItems != null && 
                            originalOrder.OrderItems.Any())
                        {
                            // Clear any existing items and manually map them
                            var orderItems = new List<OrderItemDTO>();
                            
                            foreach (var item in originalOrder.OrderItems)
                            {
                                if (item.Product != null)
                                {
                                    orderItems.Add(new OrderItemDTO
                                    {
                                        ProductId = item.Product.ProductId,
                                        ProductName = item.Product.ProductName,
                                        PictureUrl = item.Product.PictureUrl,
                                        Quantity = item.Quantity,
                                        Price = item.Price
                                    });
                                }
                            }
                            
                            // Replace the empty collection with our manually mapped items
                            mappedOrder.OrderItems = orderItems;
                            Console.WriteLine($"Manually mapped {mappedOrder.OrderItems.Count()} items for order {mappedOrder.Id}");
                        }
                    }
                }
                
                return resultList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetAllOrdersAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
                
                // Return empty list instead of throwing to avoid crashing the API
                return new List<OrderResult>();
            }
        }

        public async Task<int> UpdateOrderEmailAsync(string sourceEmail, string targetEmail)
        {
            Console.WriteLine($"OrderService.UpdateOrderEmailAsync - Transferring orders from {sourceEmail} to {targetEmail}");
            
            try
            {
                // Get the orders for the source email
                Console.WriteLine("Creating specification to find orders by source email");
                var specification = new OrderWithIncludeSpecifications(sourceEmail);
                
                Console.WriteLine("Getting orders repository");
                var orderRepo = unitOfWork.GetRepository<Order, Guid>();
                
                Console.WriteLine("Fetching orders from repository");
                var orders = await orderRepo.GetAllAsync(specification);
                
                Console.WriteLine($"Found {orders.Count()} orders for source email {sourceEmail}");
                
                if (!orders.Any())
                {
                    return 0;
                }
                
                // Update each order with the new email
                int count = 0;
                foreach (var order in orders)
                {
                    Console.WriteLine($"Updating order {order.Id} from {order.UserEmail} to {targetEmail}");
                    order.UserEmail = targetEmail;
                    orderRepo.Update(order);
                    count++;
                }
                
                // Save the changes
                Console.WriteLine($"Saving {count} updated orders");
                await unitOfWork.SaveChangesAsync();
                
                Console.WriteLine($"Successfully updated {count} orders");
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in UpdateOrderEmailAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}
