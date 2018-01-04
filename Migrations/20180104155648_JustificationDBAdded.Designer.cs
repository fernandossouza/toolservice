﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using toolservice.Data;

namespace toolservice.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180104155648_JustificationDBAdded")]
    partial class JustificationDBAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("toolservice.Model.Justification", b =>
                {
                    b.Property<int>("justificationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("text");

                    b.HasKey("justificationId");

                    b.ToTable("Justifications");
                });

            modelBuilder.Entity("toolservice.Model.StateTransitionHistory", b =>
                {
                    b.Property<int>("stateTransitionHistoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("justificationId");

                    b.Property<bool>("justificationNeeded");

                    b.Property<string>("nextState");

                    b.Property<string>("previousState");

                    b.Property<long>("timeStampTicks");

                    b.Property<int>("toolId");

                    b.HasKey("stateTransitionHistoryId");

                    b.HasIndex("justificationId");

                    b.ToTable("StateTransitionHistories");
                });

            modelBuilder.Entity("toolservice.Model.Tool", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("code")
                        .HasMaxLength(100);

                    b.Property<double>("currentLife");

                    b.Property<string>("description")
                        .HasMaxLength(100);

                    b.Property<double>("lifeCycle");

                    b.Property<string>("name")
                        .HasMaxLength(50);

                    b.Property<string>("serialNumber")
                        .HasMaxLength(100);

                    b.Property<string>("status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("available");

                    b.Property<int?>("typeId")
                        .IsRequired();

                    b.Property<string>("unitOfMeasurement")
                        .IsRequired();

                    b.HasKey("id");

                    b.HasIndex("serialNumber");

                    b.ToTable("Tools");
                });

            modelBuilder.Entity("toolservice.Model.ToolType", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("description")
                        .HasMaxLength(100);

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("status")
                        .IsRequired();

                    b.Property<int[]>("thingGroupIds")
                        .HasColumnName("thingGroupIds")
                        .HasColumnType("integer[]");

                    b.HasKey("id");

                    b.ToTable("ToolTypes");
                });

            modelBuilder.Entity("toolservice.Model.StateTransitionHistory", b =>
                {
                    b.HasOne("toolservice.Model.Justification", "justification")
                        .WithMany()
                        .HasForeignKey("justificationId");
                });
#pragma warning restore 612, 618
        }
    }
}
