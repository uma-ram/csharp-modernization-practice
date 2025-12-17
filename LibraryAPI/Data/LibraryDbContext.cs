using LibraryAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LibraryAPI.Data;

public class LibraryDbContext:DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options):base(options)
    {
        
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Book Configuration
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ISBN).IsUnique();
            entity.Property(e =>e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        //Member Configuration
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.JoinedDate).HasDefaultValueSql("GETUTCDATE()");
        });

        //Loan Configuration
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Book)
                  .WithMany(b => b.Loans)
                  .HasForeignKey(e => e.BookId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Member)
                  .WithMany(m => m.Loans)
                  .HasForeignKey(e => e.MemberId)
                  .OnDelete(DeleteBehavior.Restrict);
           

//Loan has a primary key Id

//Each loan is linked to one book and one member

//A book or member can have many loans

//Books and members cannot be deleted if loans exist
        });
    }
}
