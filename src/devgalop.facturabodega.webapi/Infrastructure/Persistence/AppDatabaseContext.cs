using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence;

public class AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : DbContext(options)
{
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmployeeEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.HiringDate).IsRequired();
            entity.Property(e => e.ContractType).HasConversion<int>().IsRequired();
            entity.Property(e => e.Status).HasConversion<int>().IsRequired();
        });

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Status).HasConversion<int>().IsRequired();
        });

        modelBuilder.Entity<PermissionEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
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

    }

}
