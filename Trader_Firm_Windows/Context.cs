using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using Trader_Firm_Windows.DataBase.Tables;

namespace Trader_Firm_Windows
{
    public class Context : DbContext
    {     
        public DbSet<users> Users { get; set; }
        public DbSet<stores> Stores { get; set; }
        public DbSet<store_products> Store_Products { get; set; }
        public DbSet<sold_products> Sold_Products { get; set; }
        public DbSet<sales> Sales { get; set; }
        public DbSet<roles> Roles { get; set; }
        public DbSet<returns> Returns { get; set; }
        public DbSet<returned_products> Returns_Products { get; set; }
        public DbSet<products> Products { get; set; }
        public DbSet<deliveries> Deliveries { get; set; }
        public DbSet<delivered_products> Delivered_Products { get; set; }
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = "Server=localhost;" +
                             "Port=5432;" +
                             "Database=TradeFirm1;" +
                             "Username=postgres;" +
                             "Password=1234;";

            optionsBuilder.UseNpgsql(connString);
        }
    }
}