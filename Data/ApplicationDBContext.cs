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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure unique constraint for the combination (not primary key)
            modelBuilder.Entity<ArticoloControlloQualita>()
                .HasIndex(a => new { a.CodArticolo, a.CodLineaProd })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}