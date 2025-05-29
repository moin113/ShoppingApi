// File: Services/IOrderService.cs
using MyntraClone.API.DTOs;

namespace MyntraClone.API.Services
{
    public interface IOrderService
    {
        Task<OrderDto> PlaceOrderAsync(CreateOrderDto dto);            // Create a new order
        Task<List<OrderDto>> GetOrdersByUserAsync(int userId);         // User’s own orders
        Task<List<OrderDto>> GetAllOrdersAsync();                      // Admin: all orders
        Task<OrderDto?> GetOrderByIdAsync(int orderId);                // Order details
    }
}
