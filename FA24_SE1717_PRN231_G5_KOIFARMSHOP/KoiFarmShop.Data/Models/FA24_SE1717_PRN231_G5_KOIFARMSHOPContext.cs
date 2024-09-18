﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KoiFarmShop.Data.Models;

public partial class FA24_SE1717_PRN231_G5_KOIFARMSHOPContext : DbContext
{
    public FA24_SE1717_PRN231_G5_KOIFARMSHOPContext()
    {
    }

    public FA24_SE1717_PRN231_G5_KOIFARMSHOPContext(DbContextOptions<FA24_SE1717_PRN231_G5_KOIFARMSHOPContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Consignment> Consignments { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<KoiFish> KoiFishes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"];
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<Consignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consignm__3214EC07D7855102");

            entity.ToTable("Consignment");

            entity.HasIndex(e => e.ConsignmentId, "UQ__Consignm__2AB758422C146C9E").IsUnique();

            entity.Property(e => e.ConsignmentId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.KoiId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Method)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Koi).WithMany(p => p.Consignments)
                .HasPrincipalKey(p => p.KoiId)
                .HasForeignKey(d => d.KoiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consignment_KoiFish");

            entity.HasOne(d => d.Payment).WithMany(p => p.Consignments)
                .HasPrincipalKey(p => p.PaymentId)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consignment_Payment");

            entity.HasOne(d => d.User).WithMany(p => p.Consignments)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consignment_User");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC073A2B2BAA");

            entity.ToTable("Image");

            entity.HasIndex(e => e.ImageId, "UQ__Image__7516F70D58F6DA7F").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.KoiId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Url)
                .IsUnicode(false)
                .HasColumnName("URL");

            entity.HasOne(d => d.Koi).WithMany(p => p.Images)
                .HasPrincipalKey(p => p.KoiId)
                .HasForeignKey(d => d.KoiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Image_KoiFish");
        });

        modelBuilder.Entity<KoiFish>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KoiFish__3214EC07558979E5");

            entity.ToTable("KoiFish");

            entity.HasIndex(e => e.KoiId, "UQ__KoiFish__E03435990A1CBF9E").IsUnique();

            entity.Property(e => e.Breed)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.KoiId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.KoiName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Origin)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC079A917FEF");

            entity.ToTable("Order");

            entity.HasIndex(e => e.OrderId, "UQ__Order__C3905BCE1DA102CD").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VoucherId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.PaymentId)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Payment");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.VoucherId)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Voucher");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC077C2BFFC4");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.KoiId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Koi).WithMany(p => p.OrderDetails)
                .HasPrincipalKey(p => p.KoiId)
                .HasForeignKey(d => d.KoiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_KoiFish");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasPrincipalKey(p => p.OrderId)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC07DABA6C94");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.PaymentId, "UQ__Payment__9B556A39A1AB3F61").IsUnique();

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07304A3D4C");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserId, "UQ__User__1788CC4D71265A60").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Voucher__3214EC07BE3A70C9");

            entity.ToTable("Voucher");

            entity.HasIndex(e => e.VoucherId, "UQ__Voucher__3AEE792029E999A4").IsUnique();

            entity.HasIndex(e => e.VoucherCode, "UQ__Voucher__7F0ABCA9EB064296").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ValidityEndDate).HasColumnType("datetime");
            entity.Property(e => e.ValidityStartDate).HasColumnType("datetime");
            entity.Property(e => e.VoucherCode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VoucherId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}