using System;
using Accounts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Accounts.Infrastructure.Migrations
{
    [DbContext(typeof(AccountsDbContext))]
    [Migration("20251228220408_InitialAccounts")]
    partial class InitialAccounts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.1");

            modelBuilder.Entity("Accounts.Entities.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("Accounts.Entities.Models.Account", b =>
                {
                    b.OwnsOne("Accounts.Entities.Models.AccountProfile", "Profile", b1 =>
                        {
                            b1.Property<Guid>("AccountId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Description")
                                .HasMaxLength(500)
                                .HasColumnType("TEXT")
                                .HasColumnName("description");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("TEXT")
                                .HasColumnName("first_name");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("TEXT")
                                .HasColumnName("last_name");

                            b1.HasKey("AccountId");

                            b1.ToTable("accounts");

                            b1.WithOwner()
                                .HasForeignKey("AccountId");
                        });

                    b.OwnsOne("SharedKernel.ValueObjects.AccountNumber", "Number", b1 =>
                        {
                            b1.Property<Guid>("AccountId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("account_number");

                            b1.HasKey("AccountId");

                            b1.HasIndex("Value")
                                .IsUnique();

                            b1.ToTable("accounts");

                            b1.WithOwner()
                                .HasForeignKey("AccountId");
                        });

                    b.Navigation("Number")
                        .IsRequired();

                    b.Navigation("Profile")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
