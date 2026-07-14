using Library.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : base(options)
    {
    }
    public DbSet<Book> Books => Set<Book>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Borrowing> Borrowings => Set<Borrowing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
            .HasIndex(x=> x.Isbn)
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasIndex(x => x.Email)
            .IsUnique();    
    }
}