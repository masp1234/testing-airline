using Microsoft.EntityFrameworkCore;
public class DatabaseContaxt : DbContext
{
    public DatabaseContaxt(DbContextOptions options) : base(options) { }


}