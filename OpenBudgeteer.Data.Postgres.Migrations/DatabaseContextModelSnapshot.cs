﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OpenBudgeteer.Data;

#nullable disable

namespace OpenBudgeteer.Data.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.Account", b =>
                {
                    b.Property<Guid>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("IsActive")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("AccountId");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BankTransaction", b =>
                {
                    b.Property<Guid>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(65, 2)");

                    b.Property<string>("Memo")
                        .HasColumnType("text");

                    b.Property<string>("Payee")
                        .HasColumnType("text");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("TransactionId");

                    b.HasIndex("AccountId");

                    b.ToTable("BankTransaction");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.Bucket", b =>
                {
                    b.Property<Guid>("BucketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BucketGroupId")
                        .HasColumnType("uuid");

                    b.Property<string>("ColorCode")
                        .HasColumnType("text");

                    b.Property<bool>("IsInactive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("IsInactiveFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("BucketId");

                    b.HasIndex("BucketGroupId");

                    b.ToTable("Bucket");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BucketGroup", b =>
                {
                    b.Property<Guid>("BucketGroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.HasKey("BucketGroupId");

                    b.ToTable("BucketGroup");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BucketMovement", b =>
                {
                    b.Property<Guid>("BucketMovementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(65, 2)");

                    b.Property<Guid>("BucketId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("MovementDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("BucketMovementId");

                    b.HasIndex("BucketId");

                    b.ToTable("BucketMovement");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BucketRuleSet", b =>
                {
                    b.Property<Guid>("BucketRuleSetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<Guid>("TargetBucketId")
                        .HasColumnType("uuid");

                    b.HasKey("BucketRuleSetId");

                    b.HasIndex("TargetBucketId");

                    b.ToTable("BucketRuleSet");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BucketVersion", b =>
                {
                    b.Property<Guid>("BucketVersionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BucketId")
                        .HasColumnType("uuid");

                    b.Property<int>("BucketType")
                        .HasColumnType("integer");

                    b.Property<int>("BucketTypeXParam")
                        .HasColumnType("integer");

                    b.Property<decimal>("BucketTypeYParam")
                        .HasColumnType("decimal(65, 2)");

                    b.Property<DateTime>("BucketTypeZParam")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("BucketVersionId");

                    b.HasIndex("BucketId");

                    b.ToTable("BucketVersion");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BudgetedTransaction", b =>
                {
                    b.Property<Guid>("BudgetedTransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(65, 2)");

                    b.Property<Guid>("BucketId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid");

                    b.HasKey("BudgetedTransactionId");

                    b.HasIndex("BucketId");

                    b.HasIndex("TransactionId");

                    b.ToTable("BudgetedTransaction");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.ImportProfile", b =>
                {
                    b.Property<Guid>("ImportProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<bool>("AdditionalSettingAmountCleanup")
                        .HasColumnType("boolean");

                    b.Property<string>("AdditionalSettingAmountCleanupValue")
                        .HasColumnType("text");

                    b.Property<int>("AdditionalSettingCreditValue")
                        .HasColumnType("integer");

                    b.Property<string>("AmountColumnName")
                        .HasColumnType("text");

                    b.Property<string>("CreditColumnIdentifierColumnName")
                        .HasColumnType("text");

                    b.Property<string>("CreditColumnIdentifierValue")
                        .HasColumnType("text");

                    b.Property<string>("CreditColumnName")
                        .HasColumnType("text");

                    b.Property<string>("DateFormat")
                        .HasColumnType("text");

                    b.Property<char>("Delimiter")
                        .HasColumnType("character(1)");

                    b.Property<int>("HeaderRow")
                        .HasColumnType("integer");

                    b.Property<string>("MemoColumnName")
                        .HasColumnType("text");

                    b.Property<string>("NumberFormat")
                        .HasColumnType("text");

                    b.Property<string>("PayeeColumnName")
                        .HasColumnType("text");

                    b.Property<string>("ProfileName")
                        .HasColumnType("text");

                    b.Property<char>("TextQualifier")
                        .HasColumnType("character(1)");

                    b.Property<string>("TransactionDateColumnName")
                        .HasColumnType("text");

                    b.HasKey("ImportProfileId");

                    b.HasIndex("AccountId");

                    b.ToTable("ImportProfile");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.MappingRule", b =>
                {
                    b.Property<Guid>("MappingRuleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BucketRuleSetId")
                        .HasColumnType("uuid");

                    b.Property<int>("ComparisionField")
                        .HasColumnType("integer");

                    b.Property<int>("ComparisionType")
                        .HasColumnType("integer");

                    b.Property<string>("ComparisionValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MappingRuleId");

                    b.HasIndex("BucketRuleSetId");

                    b.ToTable("MappingRule");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.RecurringBankTransaction", b =>
                {
                    b.Property<Guid>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(65, 2)");

                    b.Property<DateTime>("FirstOccurrenceDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Memo")
                        .HasColumnType("text");

                    b.Property<string>("Payee")
                        .HasColumnType("text");

                    b.Property<int>("RecurrenceAmount")
                        .HasColumnType("integer");

                    b.Property<int>("RecurrenceType")
                        .HasColumnType("integer");

                    b.HasKey("TransactionId");

                    b.HasIndex("AccountId");

                    b.ToTable("RecurringBankTransaction");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BankTransaction", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.Bucket", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.BucketGroup", "BucketGroup")
                        .WithMany()
                        .HasForeignKey("BucketGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BucketGroup");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BucketMovement", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.Bucket", "Bucket")
                        .WithMany()
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bucket");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BucketRuleSet", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.Bucket", "TargetBucket")
                        .WithMany()
                        .HasForeignKey("TargetBucketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TargetBucket");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BucketVersion", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.Bucket", "Bucket")
                        .WithMany()
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bucket");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.BudgetedTransaction", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.Bucket", "Bucket")
                        .WithMany()
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OpenBudgeteer.Contracts.Models.BankTransaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bucket");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.ImportProfile", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.MappingRule", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.BucketRuleSet", "BucketRuleSet")
                        .WithMany()
                        .HasForeignKey("BucketRuleSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BucketRuleSet");
                });

            modelBuilder.Entity("OpenBudgeteer.Contracts.Models.RecurringBankTransaction", b =>
                {
                    b.HasOne("OpenBudgeteer.Contracts.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });
#pragma warning restore 612, 618
        }
    }
}
