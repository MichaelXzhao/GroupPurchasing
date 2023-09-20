﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class GoShopContext : DbContext
{
    public GoShopContext(DbContextOptions<GoShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Orders> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Order__3213E83F0616E7F6");

            entity.Property(e => e.member).HasMaxLength(255);
            entity.Property(e => e.picture).HasMaxLength(255);
            entity.Property(e => e.product).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
