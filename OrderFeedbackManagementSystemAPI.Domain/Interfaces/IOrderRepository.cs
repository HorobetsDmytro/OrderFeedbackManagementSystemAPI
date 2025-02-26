using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Domain.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<List<Order>> GetUserOrdersAsync(int userId);
        Task<Order> GetOrderWithDetailsAsync(int orderId);
    }
}
