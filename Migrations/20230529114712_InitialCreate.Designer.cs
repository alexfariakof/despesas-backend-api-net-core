﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using despesas_backend_api_net_core.Infrastructure.Data.Common;

#nullable disable

namespace despesas_backend_api_net_core.Migrations
{
    [DbContext(typeof(RegisterContext))]
    [Migration("20230529114712_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Categoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Descricao")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("IdTipoCategoria")
                        .HasColumnType("int");

                    b.Property<ushort>("TipoCategoria")
                        .HasColumnType("smallint unsigned");

                    b.Property<int?>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Categoria");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.ControleAcesso", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("UsuarioId");

                    b.ToTable("ControleAcesso");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Despesa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Data")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2023, 5, 29, 8, 47, 12, 169, DateTimeKind.Local).AddTicks(5896));

                    b.Property<DateTime>("DataVencimento")
                        .HasColumnType("timestamp");

                    b.Property<string>("Descricao")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Despesa");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Lancamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Data")
                        .HasColumnType("timestamp");

                    b.Property<int?>("DespesaId")
                        .HasColumnType("int");

                    b.Property<int?>("ReceitaId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("DespesaId");

                    b.HasIndex("ReceitaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Lancamento");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Receita", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Data")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2023, 5, 29, 8, 47, 12, 169, DateTimeKind.Local).AddTicks(7535));

                    b.Property<string>("Descricao")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("IdCategoria")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.Property<decimal>("Valor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 2)")
                        .HasDefaultValue(0m);

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Receita");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("SobreNome")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<ushort>("StatusUsuario")
                        .HasColumnType("smallint unsigned");

                    b.Property<string>("Telefone")
                        .HasMaxLength(15)
                        .HasColumnType("varchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Categoria", b =>
                {
                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.ControleAcesso", b =>
                {
                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Despesa", b =>
                {
                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Categoria", "Categoria")
                        .WithMany()
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Lancamento", b =>
                {
                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Categoria", "Categoria")
                        .WithMany()
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Despesa", "Despesa")
                        .WithMany()
                        .HasForeignKey("DespesaId");

                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Receita", "Receita")
                        .WithMany()
                        .HasForeignKey("ReceitaId");

                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");

                    b.Navigation("Despesa");

                    b.Navigation("Receita");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("despesas_backend_api_net_core.Domain.Entities.Receita", b =>
                {
                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Categoria", "Categoria")
                        .WithMany()
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("despesas_backend_api_net_core.Domain.Entities.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");

                    b.Navigation("Usuario");
                });
#pragma warning restore 612, 618
        }
    }
}
