using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Labb2DatabasTest.Model;

public partial class BokhandelContext : DbContext
{
    public BokhandelContext()
    {
    }

    public BokhandelContext(DbContextOptions<BokhandelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anställda> Anställda { get; set; }

    public virtual DbSet<Beställningar> Beställningars { get; set; }

    public virtual DbSet<BeställningsDetaljer> BeställningsDetaljers { get; set; }

    public virtual DbSet<Butiker> Butikers { get; set; }

    public virtual DbSet<Böcker> Böckers { get; set; }

    public virtual DbSet<Författare> Författares { get; set; }
    public virtual DbSet<InfoPerButik> InfoPerButiks { get; set; }

    public virtual DbSet<Kunder> Kunders { get; set; }

    public virtual DbSet<LagerSaldo> LagerSaldos { get; set; }

    public virtual DbSet<Recensioner> Recensioners { get; set; }

    public virtual DbSet<TitlarPerFörfattare> TitlarPerFörfattares { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        var config = new ConfigurationBuilder().AddUserSecrets<BokhandelContext>().Build();
        var connectionString = config["ConnectionString"];
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anställda>(entity =>
        {
            entity.HasKey(e => e.AnställdId).HasName("PK__Anställd__5FC8EAC26C88D841");

            entity.Property(e => e.Namn).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Butik).WithMany(p => p.Anställda)
                .HasForeignKey(d => d.ButikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anställda__Butik__1387E197");
        });

        modelBuilder.Entity<Beställningar>(entity =>
        {
            entity.HasKey(e => e.BeställningId).HasName("PK__Beställn__DB186291C266E76B");

            entity.ToTable("Beställningar");

            entity.Property(e => e.CurrentStatus).HasMaxLength(25);
            entity.Property(e => e.TotalPris).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Butik).WithMany(p => p.Beställningars)
                .HasForeignKey(d => d.ButikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Beställni__Butik__05F8DC4F");

            entity.HasOne(d => d.Kund).WithMany(p => p.Beställningars)
                .HasForeignKey(d => d.KundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Beställni__KundI__0504B816");
        });

        modelBuilder.Entity<BeställningsDetaljer>(entity =>
        {
            entity.HasKey(e => new { e.BeställningsId, e.Isbn13 }).HasName("PK__Beställn__E02F222FDA8A314B");

            entity.ToTable("BeställningsDetaljer");

            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN13");
            entity.Property(e => e.Antal).HasDefaultValue(1);

            entity.HasOne(d => d.Beställnings).WithMany(p => p.BeställningsDetaljers)
                .HasForeignKey(d => d.BeställningsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Beställni__Order__09C96D33");

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.BeställningsDetaljers)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Beställni__ISBN1__0ABD916C");
        });

        modelBuilder.Entity<Butiker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Butiker__3214EC07C47ACC79");

            entity.ToTable("Butiker");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Adress).HasMaxLength(100);
            entity.Property(e => e.Butiksnamn).HasMaxLength(100);
            entity.Property(e => e.Hemsida).HasMaxLength(100);
            entity.Property(e => e.Land).HasMaxLength(50);
            entity.Property(e => e.Postnummer).HasMaxLength(50);
            entity.Property(e => e.Stad).HasMaxLength(100);
        });

        modelBuilder.Entity<Böcker>(entity =>
        {
            entity.HasKey(e => e.Isbn13).HasName("PK__Böcker__3BF79E03E4F677DE");

            entity.ToTable("Böcker");

            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN13");
            entity.Property(e => e.Pris).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Språk).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
        });



        modelBuilder.Entity<Författare>(entity =>
        {
            entity.HasKey(e => e.FörfattareId).HasName("PK__Författa__804114F0018A9553");

            entity.ToTable("Författare");

            entity.Property(e => e.Efternamn).HasMaxLength(50);
            entity.Property(e => e.Förnamn).HasMaxLength(50);

            entity.HasMany(d => d.Isbn13).WithMany(p => p.Författares)
                .UsingEntity<Dictionary<string, object>>(
                    "FörfattareAvBok",
                    r => r.HasOne<Böcker>().WithMany()
                        .HasForeignKey("Isbn13")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Författar__ISBN1__7ABC33CD"),
                    l => l.HasOne<Författare>().WithMany()
                        .HasForeignKey("FörfattareId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Författar__Förfa__79C80F94"),
                    j =>
                    {
                        j.HasKey("FörfattareId", "Isbn13").HasName("PK__Författa__A3FE6D10AEB72B25");
                        j.ToTable("FörfattareAvBok");
                        j.IndexerProperty<string>("Isbn13")
                            .HasMaxLength(13)
                            .IsUnicode(false)
                            .IsFixedLength()
                            .HasColumnName("ISBN13");
                    });
        });

        modelBuilder.Entity<InfoPerButik>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("InfoPerButik");

            entity.Property(e => e.AntalAnställda)
                .HasColumnName("Antal Anställda");

            entity.Property(e => e.AntalOlikaBoktitlar)
                .HasColumnName("Antal Olika Boktitlar");

            entity.Property(e => e.Butiksnamn)
                .HasMaxLength(100);

            entity.Property(e => e.Stad)
                .HasMaxLength(100);

            entity.Property(e => e.TotalLön)
                .HasColumnName("Total Lön");

            entity.Property(e => e.TotaltAntalBöcker)
                .HasColumnName("Totalt Antal Böcker");

            entity.Property(e => e.TotaltLagervärde)
                .HasColumnName("Totalt Lagervärde");
        });

        modelBuilder.Entity<Kunder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kunder__3214EC076ADBE875");

            entity.ToTable("Kunder");

            entity.HasIndex(e => e.Epost, "UQ__Kunder__0CCE4D17E84043B1").IsUnique();

            entity.Property(e => e.Epost).HasMaxLength(255);
            entity.Property(e => e.Namn).HasMaxLength(50);
            entity.Property(e => e.Telefonnummer).HasMaxLength(25);
        });

        modelBuilder.Entity<LagerSaldo>(entity =>
        {
            entity.HasKey(e => new { e.ButikId, e.Isbn13 }).HasName("PK__LagerSal__9669127AACFF78B0");

            entity.ToTable("LagerSaldo");

            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN13");
            entity.Property(e => e.AntalBöckerKvar).HasColumnName("Antal Böcker Kvar");

            entity.HasOne(d => d.Butik).WithMany(p => p.LagerSaldos)
                .HasForeignKey(d => d.ButikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LagerSaldo_Butiker");

            entity.HasOne(d => d.Böckers)
                .WithMany(p => p.LagerSaldos)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LagerSaldo_Böcker");
 
        });

        modelBuilder.Entity<Recensioner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Recensio__3214EC075F76C5A5");

            entity.ToTable("Recensioner");

            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN13");

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Recensioners)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recension__ISBN1__4460231C");

            entity.HasOne(d => d.Kund).WithMany(p => p.Recensioners)
                .HasForeignKey(d => d.KundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recension__KundI__436BFEE3");
        });

        modelBuilder.Entity<TitlarPerFörfattare>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TitlarPerFörfattare");

            entity.Property(e => e.Lagervärde)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Namn).HasMaxLength(101);
            entity.Property(e => e.Titlar)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Ålder)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
