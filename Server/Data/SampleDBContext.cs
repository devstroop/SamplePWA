using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SamplePWA.Server.Models.SampleDB;

namespace SamplePWA.Server.Data
{
    public partial class SampleDBContext : DbContext
    {
        public SampleDBContext()
        {
        }

        public SampleDBContext(DbContextOptions<SampleDBContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SamplePWA.Server.Models.SampleDB.Payment>()
              .HasOne(i => i.Customer)
              .WithMany(i => i.Payments)
              .HasForeignKey(i => i.CustomerID)
              .HasPrincipalKey(i => i.CustomerID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.Product>()
              .HasOne(i => i.ProductCategory)
              .WithMany(i => i.Products)
              .HasForeignKey(i => i.CategoryID)
              .HasPrincipalKey(i => i.CategoryID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.PurchaseOrder>()
              .HasOne(i => i.Supplier)
              .WithMany(i => i.PurchaseOrders)
              .HasForeignKey(i => i.SupplierID)
              .HasPrincipalKey(i => i.SupplierID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>()
              .HasOne(i => i.PurchaseOrder)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.OrderID)
              .HasPrincipalKey(i => i.OrderID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>()
              .HasOne(i => i.Product)
              .WithMany(i => i.PurchaseOrderDetails)
              .HasForeignKey(i => i.ProductID)
              .HasPrincipalKey(i => i.ProductID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.Sale>()
              .HasOne(i => i.Customer)
              .WithMany(i => i.Sales)
              .HasForeignKey(i => i.CustomerID)
              .HasPrincipalKey(i => i.CustomerID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.Sale>()
              .HasOne(i => i.Employee)
              .WithMany(i => i.Sales)
              .HasForeignKey(i => i.EmployeeID)
              .HasPrincipalKey(i => i.EmployeeID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.SalesDetail>()
              .HasOne(i => i.Product)
              .WithMany(i => i.SalesDetails)
              .HasForeignKey(i => i.ProductID)
              .HasPrincipalKey(i => i.ProductID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.SalesDetail>()
              .HasOne(i => i.Sale)
              .WithMany(i => i.SalesDetails)
              .HasForeignKey(i => i.SaleID)
              .HasPrincipalKey(i => i.SaleID);

            builder.Entity<SamplePWA.Server.Models.SampleDB.Payment>()
              .Property(p => p.PaymentDate)
              .HasColumnType("datetime");

            builder.Entity<SamplePWA.Server.Models.SampleDB.PurchaseOrder>()
              .Property(p => p.OrderDate)
              .HasColumnType("datetime");

            builder.Entity<SamplePWA.Server.Models.SampleDB.Sale>()
              .Property(p => p.SaleDate)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<SamplePWA.Server.Models.SampleDB.Customer> Customers { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.Employee> Employees { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.Payment> Payments { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.Product> Products { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.ProductCategory> ProductCategories { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.Sale> Sales { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.SalesDetail> SalesDetails { get; set; }

        public DbSet<SamplePWA.Server.Models.SampleDB.Supplier> Suppliers { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}