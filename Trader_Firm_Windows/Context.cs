using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using Trader_Firm_Windows.DataBase;
using Trader_Firm_Windows.DataBase.Tables;

namespace Trader_Firm_Windows
{
    public class Context : DbContext
    {
        public DbSet<users> Users { get; set; }
        public DbSet<stores> Stores { get; set; }
        public DbSet<store_products> StoreProducts { get; set; }
        public DbSet<sold_products> SoldProducts { get; set; }
        public DbSet<sales> Sales { get; set; }
        public DbSet<roles> Roles { get; set; }
        public DbSet<returns> Returns { get; set; }
        public DbSet<returned_products> ReturnsProducts { get; set; }
        public DbSet<products> Products { get; set; }
        public DbSet<deliveries> Deliveries { get; set; }
        public DbSet<delivered_products> DeliveredProducts { get; set; }
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Connection.Connect());
        }
    }
}