﻿// <auto-generated />
using System;
using FeuerwehrUpdates.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FeuerwehrUpdates.Migrations
{
    [DbContext(typeof(FWUpdatesDbContext))]
    [Migration("20230625153210_Verlauf")]
    partial class Verlauf
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.7");

            modelBuilder.Entity("FeuerwehrUpdates.DTOs.KeysDTO", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("auth")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("p256dh")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Keys");
                });

            modelBuilder.Entity("FeuerwehrUpdates.DTOs.SubscriptionDTO", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Endpoint")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long?>("ExpirationTime")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("KeysId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("KeysId")
                        .IsUnique();

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("FeuerwehrUpdates.Models.Einsatz", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("DocumentName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EinsatzId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EinsatzInfo")
                        .HasColumnType("TEXT");

                    b.Property<string>("EinsatzSchleifen")
                        .HasColumnType("TEXT");

                    b.Property<string>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<string>("PressLink")
                        .HasColumnType("TEXT");

                    b.Property<string>("StartedTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Vehicles")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Einsaetze");
                });

            modelBuilder.Entity("FeuerwehrUpdates.DTOs.SubscriptionDTO", b =>
                {
                    b.HasOne("FeuerwehrUpdates.DTOs.KeysDTO", "Keys")
                        .WithOne("Subscription")
                        .HasForeignKey("FeuerwehrUpdates.DTOs.SubscriptionDTO", "KeysId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Keys");
                });

            modelBuilder.Entity("FeuerwehrUpdates.DTOs.KeysDTO", b =>
                {
                    b.Navigation("Subscription")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
