using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<DatabaseRecord> DatabaseRecords { get; set; }
        public DbSet<TipologiaSignificato> TipologieSignificati { get; set; }
        public DbSet<ArticoloControlloQualita> ArticoliControlloQualita { get; set; }
        public DbSet<DispositivoMultimediale> DispositiviMultimediali { get; set; }
        public DbSet<Acquisizione> Acquisizioni { get; set; }
        public DbSet<VAgLineePostazioni> VAgLineePostazioni { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure unique constraint for the combination (not primary key)
            modelBuilder.Entity<ArticoloControlloQualita>()
                .HasIndex(a => new { a.CodArticolo, a.CodLineaProd })
                .IsUnique();

            // Configure unique constraint for the combination of production line, workstation and serial number
            modelBuilder.Entity<DispositivoMultimediale>()
                .HasIndex(d => new { d.CodLineaProd, d.CodPostazione, d.SerialeDispositivo })
                .IsUnique();

            // Configure the V_AG_LINEE_POSTAZIONI view
            modelBuilder.Entity<VAgLineePostazioni>()
                .HasNoKey()
                .ToView("V_AG_LINEE_POSTAZIONI");

            base.OnModelCreating(modelBuilder);
        }
    }
}