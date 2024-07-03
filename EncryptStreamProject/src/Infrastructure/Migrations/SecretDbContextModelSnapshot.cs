﻿// <auto-generated />
using System;
using System.Diagnostics.CodeAnalysis;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [ExcludeFromCodeCoverage]
    [DbContext(typeof(SecretDbContext))]
    partial class SecretDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<int>("EncryptStatus")
                        .HasColumnType("integer");

                    b.Property<string>("TextEncrypted")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Secrets", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Secret", b =>
                {
                    b.OwnsOne("Domain.Model.SecretEncryptData", "SecretEncryptData", b1 =>
                        {
                            b1.Property<string>("SecretId")
                                .HasColumnType("text");

                            b1.Property<int>("EncryptType")
                                .HasColumnType("integer");

                            b1.Property<string>("KeyValue")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("SecretId");

                            b1.ToTable("Secrets");

                            b1.WithOwner()
                                .HasForeignKey("SecretId");
                        });

                    b.Navigation("SecretEncryptData")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
