﻿// <auto-generated />
using System;
using DayForceIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DayForceIntegration.Migrations
{
    [DbContext(typeof(DBDataContext))]
    [Migration("20230203101246_initital")]
    partial class initital
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DayForceIntegration.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("LastRefreshDatetime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LicenceValidUntil")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("settings");
                });
#pragma warning restore 612, 618
        }
    }
}
