using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Domain.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetOrderReviewsAsync(int orderId);
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<IEnumerable<Review>> GetFilteredReviewsAsync(int rating);
        Task<double> GetProductAverageRatingAsync(int productId);
        Task<Review> GetReviewByOrderIdAsync(int orderId);
        Task<Review> GetReviewByOrderAndProductAsync(int orderId, int productId);
    }
}
