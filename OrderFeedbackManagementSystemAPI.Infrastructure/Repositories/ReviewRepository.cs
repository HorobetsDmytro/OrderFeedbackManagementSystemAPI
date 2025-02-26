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
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetOrderReviewsAsync(int orderId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<double> GetProductAverageRatingAsync(int productId)
        {
            var reviews = await _context.Reviews.Where(r => r.ProductId == productId).ToListAsync();
            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<Review> GetReviewByOrderIdAsync(int orderId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        public async Task<IEnumerable<Review>> GetFilteredReviewsAsync(int rating)
        {
            return await _context.Reviews
                .Where(r => r.Rating == rating)
                .ToListAsync();
        }
        
        public async Task<Review> GetReviewByOrderAndProductAsync(int orderId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.OrderId == orderId && r.ProductId == productId);
        }
        
        public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User) // Завантажуємо дані користувача
                .Where(r => r.Order.OrderItems.Any(oi => oi.ProductId == productId))
                .ToListAsync();
        }
    }
}
