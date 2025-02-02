using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task UpdateStockAsync(int productId, int quantity);
    }
}
