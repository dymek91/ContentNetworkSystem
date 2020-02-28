﻿// <auto-generated />
using System;
using ContentNetworkSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContentNetworkSystem.Migrations
{
    [DbContext(typeof(ContentNetworkSystemContext))]
    [Migration("20200228153936_AddedWasSuccess")]
    partial class AddedWasSuccess
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ContentNetworkSystem.Models.Content", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<string>("TypeName")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("ProjectId")
                        .IsUnique();

                    b.ToTable("Content");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Content");
                });

            modelBuilder.Entity("ContentNetworkSystem.Models.Group", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Group");
                });

            modelBuilder.Entity("ContentNetworkSystem.Models.Project", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("DateAdded")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<TimeSpan>("Frequency")
                        .HasColumnType("interval");

                    b.Property<int?>("GroupId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastPushed")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("WasSuccess")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.HasKey("ID");

                    b.HasIndex("GroupId");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("ContentNetworkSystem.Models.Scheduler", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("RequestId")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("Scheduler");
                });

            modelBuilder.Entity("ContentNetworkSystem.Models.Wordpress", b =>
                {
                    b.HasBaseType("ContentNetworkSystem.Models.Content");

                    b.Property<int?>("BlogId")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<int?>("TextGenerationCategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("Wordpress");
                });

            modelBuilder.Entity("ContentNetworkSystem.Models.Content", b =>
                {
                    b.HasOne("ContentNetworkSystem.Models.Project", "Project")
                        .WithOne("Content")
                        .HasForeignKey("ContentNetworkSystem.Models.Content", "ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContentNetworkSystem.Models.Project", b =>
                {
                    b.HasOne("ContentNetworkSystem.Models.Group", "Group")
                        .WithMany("Projects")
                        .HasForeignKey("GroupId");
                });
#pragma warning restore 612, 618
        }
    }
}
