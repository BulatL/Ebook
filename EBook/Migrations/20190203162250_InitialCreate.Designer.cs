﻿// <auto-generated />
using System;
using EBook.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EBook.Migrations
{
    [DbContext(typeof(EbookDbContext))]
    [Migration("20190203162250_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EBook.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<string>("Body");

                    b.Property<int>("CategoryId");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Keywords")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<int>("LanguageId");

                    b.Property<string>("MIME")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("PublicationYear");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(80);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("LanguageId");

                    b.ToTable("Book");
                });

            modelBuilder.Entity("EBook.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("Category");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Action and Adventure"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Art"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Biography"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Comic book"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Crime"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Drama"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Fantasy"
                        },
                        new
                        {
                            Id = 8,
                            Name = "History"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Horror"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Mystery"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Review"
                        },
                        new
                        {
                            Id = 12,
                            Name = "Political thriller"
                        },
                        new
                        {
                            Id = 13,
                            Name = "Science"
                        },
                        new
                        {
                            Id = 14,
                            Name = "Romance"
                        },
                        new
                        {
                            Id = 15,
                            Name = "Science fiction"
                        },
                        new
                        {
                            Id = 16,
                            Name = "True crime"
                        },
                        new
                        {
                            Id = 17,
                            Name = "Short story	"
                        },
                        new
                        {
                            Id = 18,
                            Name = "Thriller	"
                        });
                });

            modelBuilder.Entity("EBook.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("Language");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Spanish"
                        },
                        new
                        {
                            Id = 2,
                            Name = "English"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Russian"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Japanese"
                        },
                        new
                        {
                            Id = 5,
                            Name = "German"
                        },
                        new
                        {
                            Id = 6,
                            Name = "French"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Turkish"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Italian"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Polish"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Ukrainian"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Serbian"
                        },
                        new
                        {
                            Id = 12,
                            Name = "Hungarian"
                        },
                        new
                        {
                            Id = 13,
                            Name = "Greek"
                        },
                        new
                        {
                            Id = 14,
                            Name = "Czech"
                        });
                });

            modelBuilder.Entity("EBook.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<int>("Role");

                    b.Property<int?>("SubscribedCategorieId");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("SubscribedCategorieId");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Firstname = "Marko",
                            Lastname = "Markovic",
                            Password = "123",
                            Role = 0,
                            Username = "marko"
                        },
                        new
                        {
                            Id = 2,
                            Firstname = "Nikola",
                            Lastname = "Nikolic",
                            Password = "123",
                            Role = 0,
                            SubscribedCategorieId = 1,
                            Username = "nikola"
                        },
                        new
                        {
                            Id = 3,
                            Firstname = "Ivan",
                            Lastname = "Ivanovic",
                            Password = "123",
                            Role = 1,
                            SubscribedCategorieId = 2,
                            Username = "ivan"
                        });
                });

            modelBuilder.Entity("EBook.Models.Book", b =>
                {
                    b.HasOne("EBook.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EBook.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("EBook.Models.User", b =>
                {
                    b.HasOne("EBook.Models.Category", "SubscribedCategorie")
                        .WithMany()
                        .HasForeignKey("SubscribedCategorieId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
