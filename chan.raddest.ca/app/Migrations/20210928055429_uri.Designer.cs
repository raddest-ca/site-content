﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using app.Data;

namespace app.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210928055429_uri")]
    partial class uri
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("app.Models.File", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Uri")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("app.Models.Post", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuthorHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorIp")
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileId")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("app.Models.Post", b =>
                {
                    b.HasOne("app.Models.File", "File")
                        .WithMany()
                        .HasForeignKey("FileId");

                    b.Navigation("File");
                });
#pragma warning restore 612, 618
        }
    }
}
