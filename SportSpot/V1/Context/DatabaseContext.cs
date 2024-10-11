using Microsoft.EntityFrameworkCore;
using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Context
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {

        public DbSet<MediaEntity> Media { get; set; }

    }
}
