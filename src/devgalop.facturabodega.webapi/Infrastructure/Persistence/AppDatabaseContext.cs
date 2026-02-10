using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Invoices.Common;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Features.Users.Customers.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence;

public class AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : DbContext(options)
{
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<EmployeeRefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductStockEntity> ProductStocks { get; set; }
    public DbSet<RecoverPasswordTokenEntity> RecoverPasswordTokens { get; set; }
    public DbSet<InvoiceEntity> Invoices { get; set; }
    public DbSet<InvoiceDetailEntity> InvoiceDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmployeeEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHashed).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Document).IsRequired().HasMaxLength(50);
            entity.Property(e => e.HiringDate).IsRequired();
            entity.Property(e => e.ContractType).HasConversion<int>().IsRequired();
            entity.Property(e => e.Status).HasConversion<int>().IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Document).IsUnique();
        });

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Status).HasConversion<int>().IsRequired();
            entity.HasIndex(r => r.Name).IsUnique();
        });

        modelBuilder.Entity<PermissionEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(p => p.Name).IsUnique();
        });

        modelBuilder.Entity<EmployeeRefreshTokenEntity>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(200);
            entity.Property(rt => rt.ExpiresOnUtc).IsRequired();
            entity.HasIndex(rt => rt.Token).IsUnique();
        });

        modelBuilder.Entity<CustomerEntity>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Document).IsRequired().HasMaxLength(50);
            entity.HasIndex(c => c.Email).IsUnique();
            entity.HasIndex(c => c.Document).IsUnique();
        });

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.UnitPrice).IsRequired();
            entity.HasIndex(p => p.Name).IsUnique();
            entity.HasOne(p => p.Stock)
                  .WithOne(ps => ps.Product)
                  .HasForeignKey<ProductStockEntity>(ps => ps.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductStockEntity>(entity =>
        {
            entity.HasKey(ps => ps.Id);
            entity.Property(ps => ps.Quantity).IsRequired();
            entity.HasOne(ps => ps.Product)
                  .WithOne(ps => ps.Stock)
                  .HasForeignKey<ProductEntity>(ps => ps.StockId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecoverPasswordTokenEntity>(entity =>
        {
            entity.HasKey(rpt => rpt.Id);
            entity.Property(rpt => rpt.Token).IsRequired().HasMaxLength(200);
            entity.Property(rpt => rpt.ExpiresOnUtc).IsRequired();
            entity.Property(rpt => rpt.IsUsed).IsRequired();
            entity.HasIndex(rpt => rpt.Token).IsUnique();

            entity.HasOne(rpt => rpt.Employee)
                    .WithMany()
                    .HasForeignKey(rpt => rpt.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceEntity>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.DateOnUtc).IsRequired();
            entity.Property(i => i.ClientId).IsRequired();
            entity.HasOne(i => i.Client)
                  .WithMany(c => c.Invoices)
                  .HasForeignKey(i => i.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceDetailEntity>(entity =>
        {
            entity.HasKey(id => id.Id);
            entity.Property(id => id.Quantity).IsRequired();
            entity.Property(id => id.ProductId).IsRequired();
            entity.Property(id => id.InvoiceId).IsRequired();

            entity.HasOne(id => id.Product)
                  .WithMany()
                  .HasForeignKey(id => id.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(id => id.Invoice)
                  .WithMany(i => i.Details)
                  .HasForeignKey(id => id.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Relación 1-N entre Employee y Role
        modelBuilder.Entity<EmployeeEntity>()
            .HasOne(e => e.Role) // Un empleado tiene un rol
            .WithMany()
            .HasForeignKey(e => e.RoleId) // La clave foránea en EmployeeEntity
            .OnDelete(DeleteBehavior.Restrict); // Evita la eliminación en cascada

        // Relación N-N entre Role y Permission
        modelBuilder.Entity<RoleEntity>()
            .HasMany(r => r.Permissions) // Un rol tiene muchos permisos
            .WithMany(p => p.RolesAssociated) // Un permiso está asociado a muchos roles
            .UsingEntity<Dictionary<string, object>>( // Tabla intermedia
                "RolePermission", // Nombre de la tabla intermedia
                r => r.HasOne<PermissionEntity>() // Configuración de la relación con PermissionEntity
                      .WithMany()
                      .HasForeignKey("PermissionId")
                      .OnDelete(DeleteBehavior.Cascade), // Eliminación en cascada
                p => p.HasOne<RoleEntity>() // Configuración de la relación con RoleEntity
                      .WithMany()
                      .HasForeignKey("RoleId")
                      .OnDelete(DeleteBehavior.Cascade)); // Eliminación en cascada

        // Relación 1-N entre Employee y EmployeeRefreshTokenEntity
        modelBuilder.Entity<EmployeeRefreshTokenEntity>()
            .HasOne(rt => rt.Employee) // Un token de refresco pertenece a un empleado
            .WithMany(e => e.RefreshTokens) // Un empleado puede tener muchos tokens de refresco
            .HasForeignKey(rt => rt.EmployeeId) // La clave foránea en EmployeeRefreshTokenEntity
            .OnDelete(DeleteBehavior.Cascade); // Eliminación en cascada

    }

}
