﻿// <auto-generated />
using System;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(SecretDbContext))]
    [Migration("20240527021039_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Model.Secret", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TextEncrypted")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Secrets", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Secret", b =>
                {
                    b.OwnsOne("Domain.Model.HashCryptor", "HashCryptor", b1 =>
                        {
                            b1.Property<string>("SecretId")
                                .HasColumnType("text");

                            b1.HasKey("SecretId");

                            b1.ToTable("HashCryptors");

                            b1.WithOwner()
                                .HasForeignKey("SecretId");
                        });

                    b.Navigation("HashCryptor")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
