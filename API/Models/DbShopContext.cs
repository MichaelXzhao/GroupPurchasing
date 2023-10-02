using System;
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
            entity.HasKey(e => e.id).HasName("PK__Orders__3213E83FD5604FF5");

            entity.Property(e => e.campaign).HasMaxLength(255);
            entity.Property(e => e.finish).HasColumnType("date");
            entity.Property(e => e.member).HasMaxLength(255);
            entity.Property(e => e.picture).HasMaxLength(255);
            entity.Property(e => e.product).HasMaxLength(255);
            entity.Property(e => e.promotionid).HasMaxLength(255);
            entity.Property(e => e.recommender).HasMaxLength(255);
            entity.Property(e => e.salepageid).HasMaxLength(255);
            entity.Property(e => e.shopid).HasMaxLength(255);
            entity.Property(e => e.skuid).HasMaxLength(255);
            entity.Property(e => e.start).HasColumnType("date");
            entity.Property(e => e.status).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
