using System;
using System.Collections.Generic;
using Cwiczenia10.Models;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia10.Data;

public partial class UsserContext : DbContext
{
    public UsserContext()
    {
    }

    public UsserContext(DbContextOptions<UsserContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Default");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.Property(e => e.IdUser).ValueGeneratedOnAdd();
            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RefreshToken).HasMaxLength(2000);
            entity.Property(e => e.Salt).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
