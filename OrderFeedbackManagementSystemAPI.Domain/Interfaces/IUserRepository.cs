using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
    }
}
