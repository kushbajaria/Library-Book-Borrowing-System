using LibraryBookBorrowingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBookBorrowingSystem.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BorrowRecord>(entity =>
        {
            entity.HasOne(r => r.Book)
                .WithMany(b => b.BorrowRecords)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Member)
                .WithMany(m => m.BorrowRecords)
                .HasForeignKey(r => r.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}