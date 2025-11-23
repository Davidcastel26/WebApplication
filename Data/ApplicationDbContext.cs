using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<TipoGasto> TipoGastos { get; set; } = null!;
    public DbSet<FondoMonetario> FondoMonetarios { get; set; } = null!;
    public DbSet<Presupuesto> Presupuestos { get; set; } = null!;
    public DbSet<GastoEncabezado> GastoEncabezados { get; set; } = null!;
    public DbSet<GastoDetalle> GastoDetalles { get; set; } = null!;
    public DbSet<Deposito> Depositos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TIPO GASTO
        modelBuilder.Entity<TipoGasto>(e =>
        {
            e.ToTable("TipoGasto");
            e.HasKey(x => x.Id);

            e.Property(x => x.Codigo)
                .IsRequired()
                .HasMaxLength(20);

            e.HasIndex(x => x.Codigo)
                .IsUnique();

            e.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(x => x.Descripcion)
                .HasMaxLength(300);
        });

        // FONDO MONETARIO
        modelBuilder.Entity<FondoMonetario>(e =>
        {
            e.ToTable("FondoMonetario");
            e.HasKey(x => x.Id);

            e.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(x => x.TipoFondo)
                .HasConversion<string>()   // Enum como texto
                .IsRequired();

            e.Property(x => x.NumeroCuenta)
                .HasMaxLength(50);

            e.Property(x => x.Descripcion)
                .HasMaxLength(300);

            // (Opcional) evitar fondos duplicados por nombre
            e.HasIndex(x => x.Nombre).IsUnique(false);
        });

        // PRESUPUESTO (por mes/año/tipo/usuario)
        modelBuilder.Entity<Presupuesto>(e =>
        {
            e.ToTable("Presupuesto", t =>
            {
                t.HasCheckConstraint("CK_Presupuesto_Mes", "[Mes] BETWEEN 1 AND 12");
                t.HasCheckConstraint("CK_Presupuesto_MontoPositivo", "[MontoPresupuestado] >= 0");
            });

            e.HasKey(x => x.Id);

            e.Property(x => x.Mes).IsRequired();

            e.Property(x => x.Anio).IsRequired();

            e.Property(x => x.MontoPresupuestado)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Unicidad: (Anio, Mes, TipoGastoId, UsuarioId)
            e.HasIndex(x => new { x.Anio, x.Mes, x.TipoGastoId, x.UsuarioId })
                .IsUnique();

            e.HasOne(x => x.TipoGasto)
                .WithMany(t => t.Presupuestos)
                .HasForeignKey(x => x.TipoGastoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GASTO ENCABEZADO
        modelBuilder.Entity<GastoEncabezado>(e =>
        {
            e.ToTable("GastoEncabezado");
            e.HasKey(x => x.Id);

            e.Property(x => x.Fecha)
                .IsRequired();

            e.Property(x => x.Observaciones)
                .HasMaxLength(500);

            e.Property(x => x.NombreComercio)
                .HasMaxLength(150);

            e.Property(x => x.TipoDocumento)
                .HasConversion<string>()   // Enum como texto
                .IsRequired();

            e.HasOne(x => x.FondoMonetario)
                .WithMany(f => f.Gastos)
                .HasForeignKey(x => x.FondoMonetarioId)
                .OnDelete(DeleteBehavior.Restrict); // mantener historial si se elimina fondo

            // Importante: No se puede forzar "al menos 1 detalle" con FK.
            // Eso se valida a nivel de servicio/SaveChanges (transacción).
        });

        
        // GASTO DETALLE
        modelBuilder.Entity<GastoDetalle>(e =>
        {
                e.ToTable("GastoDetalle", t =>
                {
                    t.HasCheckConstraint("CK_GastoDetalle_MontoPositivo", "[Monto] > 0");
                });

                e.HasKey(x => x.Id);

                e.Property(x => x.Monto)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                e.HasOne(x => x.GastoEncabezado)
                    .WithMany(eh => eh.Detalles)
                    .HasForeignKey(x => x.GastoEncabezadoId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.TipoGasto)
                    .WithMany(t => t.GastoDetalles)
                    .HasForeignKey(x => x.TipoGastoId)
                    .OnDelete(DeleteBehavior.Restrict);
        });

        // DEPOSITO
        modelBuilder.Entity<Deposito>(e =>
        {
                e.ToTable("Deposito", t =>
                {
                    t.HasCheckConstraint("CK_Deposito_MontoPositivo", "[Monto] > 0");
                });

                e.HasKey(x => x.Id);

                e.Property(x => x.Fecha)
                    .IsRequired();

                e.Property(x => x.Monto)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                e.HasOne(x => x.FondoMonetario)
                    .WithMany(f => f.Depositos)
                    .HasForeignKey(x => x.FondoMonetarioId)
                    .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

