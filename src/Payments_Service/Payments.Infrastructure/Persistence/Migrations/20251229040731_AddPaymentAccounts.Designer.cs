using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Payments.Infrastructure.Persistence;

#nullable disable

namespace Payments.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(PaymentsDbContext))]
    [Migration("20251229040731_AddPaymentAccounts")]
    partial class AddPaymentAccounts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.1");

            modelBuilder.Entity("Payments.Entities.Models.InboxMessage", b =>
                {
                    b.Property<Guid>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ProcessedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ReceivedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("MessageId");

                    b.ToTable("inbox_messages", (string)null);
                });

            modelBuilder.Entity("Payments.Entities.Models.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Attempts")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ProcessedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Status");

                    b.ToTable("outbox_messages", (string)null);
                });

            modelBuilder.Entity("Payments.Entities.Models.PaymentAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<long>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("payment_accounts", (string)null);
                });

            modelBuilder.Entity("Payments.Entities.Models.PaymentOperation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reason")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("payment_operations", (string)null);
                });

            modelBuilder.Entity("Payments.Entities.Models.PaymentAccount", b =>
                {
                    b.OwnsOne("SharedKernel.ValueObjects.AccountNumber", "AccountNumber", b1 =>
                        {
                            b1.Property<Guid>("PaymentAccountId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("account_number");

                            b1.HasKey("PaymentAccountId");

                            b1.HasIndex("Value")
                                .IsUnique();

                            b1.ToTable("payment_accounts");

                            b1.WithOwner()
                                .HasForeignKey("PaymentAccountId");
                        });

                    b.Navigation("AccountNumber")
                        .IsRequired();
                });

            modelBuilder.Entity("Payments.Entities.Models.PaymentOperation", b =>
                {
                    b.OwnsOne("SharedKernel.ValueObjects.AccountNumber", "AccountNumber", b1 =>
                        {
                            b1.Property<Guid>("PaymentOperationId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("account_number");

                            b1.HasKey("PaymentOperationId");

                            b1.ToTable("payment_operations");

                            b1.WithOwner()
                                .HasForeignKey("PaymentOperationId");
                        });

                    b.Navigation("AccountNumber")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
