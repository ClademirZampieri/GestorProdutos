﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestorProdutos.Catalogo.Domain;

namespace GestorProdutos.Front.Produtos.Data.Mappings
{
    public class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasColumnType("varchar(250)");

            builder.Property(c => c.Imagem)
                .IsRequired()
                .HasColumnType("varchar(250)");

            builder.Property(c => c.StatusSincronizacao)
                .IsRequired()
                .HasColumnType("int16");

            builder.ToTable("Produtos");
        }
    }
}