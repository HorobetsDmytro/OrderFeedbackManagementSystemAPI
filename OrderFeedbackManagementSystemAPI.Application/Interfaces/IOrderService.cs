using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Enums;

namespace OrderFeedbackManagementSystemAPI.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int userId, List<OrderItem> items);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<decimal> CalculateOrderTotalAsync(List<OrderItem> items);
    }
}
