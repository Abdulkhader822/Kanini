using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DB_First_Handson.Models;

public partial class TravelBuddyDbContext : DbContext
{
    public TravelBuddyDbContext()
    {
    }

    public TravelBuddyDbContext(DbContextOptions<TravelBuddyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TravelExperience> TravelExperiences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=ABDULKHADER67;database=TravelBuddyDB;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TravelExperience>(entity =>
        {
            entity.HasKey(e => e.ExperienceId).HasName("PK__TravelEx__2F4E34691FA6CB4E");

            entity.Property(e => e.ExperienceId).HasColumnName("ExperienceID");
            entity.Property(e => e.Destination).HasMaxLength(100);
            entity.Property(e => e.IsSoloTravel).HasDefaultValue(false);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
