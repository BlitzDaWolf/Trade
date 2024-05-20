﻿// <auto-generated />
using System;
using Command.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Command.Migrations
{
    [DbContext(typeof(CommandContext))]
    partial class CommandContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Command.Models.Price", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<double>("Close")
                        .HasColumnType("REAL");

                    b.Property<double>("High")
                        .HasColumnType("REAL");

                    b.Property<double>("Low")
                        .HasColumnType("REAL");

                    b.Property<double>("Open")
                        .HasColumnType("REAL");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("TickVolume")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("Command.Models.ResultModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<double>("PnL")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("Command.Models.StrategyArgumentModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Argument")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ResultModelId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Strategy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("StrategyArguments");
                });
#pragma warning restore 612, 618
        }
    }
}
