﻿// <auto-generated />
using System;
using FeintSite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FeintSite.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20200611203048_Trip")]
    partial class Trip
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("FeintSDK.SessionKey", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<int>("SessionKeyId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("SessionKey");
                });

            modelBuilder.Entity("FeintSDK.SessionProperty", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<int>("SessionPropertyId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("SessionProperty");
                });

            modelBuilder.Entity("FeintSite.Models.ExampleModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ExamplePropertyInteger")
                        .HasColumnType("integer");

                    b.Property<string>("ExamplePropertyString")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ExampleModel");
                });

            modelBuilder.Entity("FeintSite.Models.TripModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("TripModel");
                });

            modelBuilder.Entity("FeintSite.Models.TripPointModel", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Point>("Location")
                        .HasColumnType("geometry");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("TripId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TripId");

                    b.ToTable("TripPointModel");
                });

            modelBuilder.Entity("FeintSDK.SessionProperty", b =>
                {
                    b.HasOne("FeintSDK.SessionKey", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("FeintSite.Models.TripPointModel", b =>
                {
                    b.HasOne("FeintSite.Models.TripModel", "Trip")
                        .WithMany()
                        .HasForeignKey("TripId");
                });
#pragma warning restore 612, 618
        }
    }
}
