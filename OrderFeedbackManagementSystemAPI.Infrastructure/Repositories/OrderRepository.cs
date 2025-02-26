using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Interfaces;
using OrderFeedbackManagementSystemAPI.Infrastructure.Data;

namespace OrderFeedbackManagementSystemAPI.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
