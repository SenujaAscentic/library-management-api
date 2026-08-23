using Library.Application.Abstractions;
using Library.Infrastructure.Data;

namespace Library.Infrastructure;

public class UnitOfWork(LibraryDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}