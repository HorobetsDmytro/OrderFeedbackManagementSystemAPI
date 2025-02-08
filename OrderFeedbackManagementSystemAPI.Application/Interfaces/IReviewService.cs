using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Enums;

namespace OrderFeedbackManagementSystemAPI.Application.Interfaces
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(int userId, int orderId, int rating, string comment);
        Task<Review> UpdateReviewAsync(int reviewId, int rating, string comment);
        Task<IEnumerable<Review>> GetOrderReviewsAsync(int orderId);
        Task<IEnumerable<Review>> GetFilteredReviewsAsync(int rating);
        Task<double> GetProductAverageRatingAsync(int productId);
        Task<Review> HandleOrderStatusChangeAsync(int orderId, OrderStatus newStatus);

    }
}
