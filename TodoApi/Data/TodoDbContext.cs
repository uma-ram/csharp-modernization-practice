namespace TodoApi.Data;

using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
public class TodoDbContext:DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext > options):base(options)
    {
        
    }
    public DbSet<TodoItem> TodoItems {get;set;} =null!;
}
