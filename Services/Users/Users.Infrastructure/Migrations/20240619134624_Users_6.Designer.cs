﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Infrastructure.Data;

#nullable disable

namespace Users.Infrastructure.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20240619134624_Users_6")]
    partial class Users_6
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Users.Core.Entities.IpConnection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsHardDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("Modified")
                        .HasColumnType("datetime(6)");

                    b.Property<uint?>("ModifiedCount")
                        .HasColumnType("int unsigned");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("IpConnection");
                });

            modelBuilder.Entity("Users.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<uint>("Code")
                        .HasColumnType("int unsigned");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsHardDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("Modified")
                        .HasColumnType("datetime(6)");

                    b.Property<uint?>("ModifiedCount")
                        .HasColumnType("int unsigned");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Users.Core.Entities.IpConnection", b =>
                {
                    b.HasOne("Users.Core.Entities.User", null)
                        .WithMany("IpConnections")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Users.Core.Entities.User", b =>
                {
                    b.Navigation("IpConnections");
                });
#pragma warning restore 612, 618
        }
    }
}
