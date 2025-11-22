using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TbspRpgDataLayer
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            builder.UseSqlite(connectionString);

            return new DatabaseContext(builder.Options);
        }

        public DatabaseContext CreateDbContext(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseSqlite(connectionString);
            return new DatabaseContext(builder.Options);
        }
    }
}