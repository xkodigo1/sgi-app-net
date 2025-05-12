using Microsoft.EntityFrameworkCore;
using sgi_app.domain.entities;
using System;

namespace sgi_app.infrastructure.sql
{
    public class YourDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Terceros> Terceros { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<MovCaja> MovCaja { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<TipoTercero> TipoTerceros { get; set; }
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<PlanProducto> PlanProductos { get; set; }
        public DbSet<EPS> EPS { get; set; }
        public DbSet<ARL> ARL { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=sgi-db;User=sgiapp;Password=kodigo777;SslMode=none;",
                new MySqlServerVersion(new Version(8, 0, 21)),
                options => options.EnableRetryOnFailure()
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlanProducto>()
                .HasKey(pp => new { pp.PlanId, pp.ProductoId });
                
            modelBuilder.Entity<EPS>().ToTable("EPS");
            modelBuilder.Entity<ARL>().ToTable("ARL");
        }
    }
} 