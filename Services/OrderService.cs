using Microsoft.EntityFrameworkCore;
using MyntraClone.API.Data;
using MyntraClone.API.DTOs;
using MyntraClone.API.Models;

namespace MyntraClone.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Place a new order
        public async Task<OrderDto> PlaceOrderAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.UtcNow,  // ✅ Fixed
                ShippingAddress = dto.ShippingAddress,
                PaymentReference = dto.PaymentReference,  // ✅ Added
                PaymentIntentId = dto.PaymentIntentId,
                OrderItems = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            order.TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);  // ✅ Updated

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                PaymentReference = order.PaymentReference,
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = "", // Optional: Load via join
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                TotalAmount = order.TotalAmount
            };
        }

        // 2. Get orders for a user
        public async Task<List<OrderDto>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    ShippingAddress = o.ShippingAddress,
                    PaymentReference = o.PaymentReference,
                    OrderItems = o.OrderItems.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = "",
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList(),
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();
        }

        // 3. Admin: Get all orders
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    ShippingAddress = o.ShippingAddress,
                    PaymentReference = o.PaymentReference,
                    OrderItems = o.OrderItems.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = "",
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList(),
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();
        }

        // 4. Get a single order
        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var o = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (o == null) return null;

            return new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                PaymentReference = o.PaymentReference,
                OrderItems = o.OrderItems.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = "",
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                TotalAmount = o.TotalAmount
            };
        }
    }
}
