
namespace Library.Application.Abstractions.Repositories;

using Library.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}