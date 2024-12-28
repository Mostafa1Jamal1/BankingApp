﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using api.Models;

#nullable disable

namespace api.Migrations
{
    [DbContext(typeof(BankingDbContext))]
    [Migration("20241228144854_smallChange")]
    partial class smallChange
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("api.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<string>("AccountType")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.HasKey("Id");

                    b.HasIndex("AccountNumber")
                        .IsUnique();

                    b.ToTable("Accounts", (string)null);

                    b.HasDiscriminator<string>("AccountType").HasValue("Base");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("api.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AddedTo")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("DeductedFrom")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.ToTable("Transactions", (string)null);
                });

            modelBuilder.Entity("api.Models.CheckingAccount", b =>
                {
                    b.HasBaseType("api.Models.Account");

                    b.Property<decimal>("OverdraftLimit")
                        .HasColumnType("decimal(18,2)");

                    b.HasDiscriminator().HasValue("Checking");

                    b.HasData(
                        new
                        {
                            Id = 2,
                            AccountNumber = "7777777777",
                            Balance = 100000.00m,
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Currency = "USD",
                            Name = "Mostafa Personal",
                            Status = "Active",
                            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            OverdraftLimit = 500m
                        });
                });

            modelBuilder.Entity("api.Models.SavingsAccount", b =>
                {
                    b.HasBaseType("api.Models.Account");

                    b.Property<decimal>("InterestRate")
                        .HasColumnType("decimal(6,3)");

                    b.HasDiscriminator().HasValue("Savings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AccountNumber = "6666666666",
                            Balance = 500000.00m,
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Currency = "USD",
                            Name = "Mostafa Corp",
                            Status = "Active",
                            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            InterestRate = 0.15m
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
