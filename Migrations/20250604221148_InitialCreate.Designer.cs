﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RunTrackerAPI.Models;

#nullable disable

namespace RunTrackerAPI.Migrations
{
    [DbContext(typeof(RunDb))]
    [Migration("20250604221148_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("RunTrackerAPI.Models.Duration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Hours")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Minutes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Seconds")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Durations");
                });

            modelBuilder.Entity("RunTrackerAPI.Models.Run", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DurationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Label")
                        .HasColumnType("TEXT");

                    b.Property<double>("Mileage")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("DurationId");

                    b.ToTable("RunData");
                });

            modelBuilder.Entity("RunTrackerAPI.Models.Run", b =>
                {
                    b.HasOne("RunTrackerAPI.Models.Duration", "Duration")
                        .WithMany()
                        .HasForeignKey("DurationId");

                    b.Navigation("Duration");
                });
#pragma warning restore 612, 618
        }
    }
}
