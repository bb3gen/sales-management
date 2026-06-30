using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagement.Api.Domain.Entities;

namespace SalesManagement.Api.Features.Orders.Configurations;

public sealed class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // orders に対してマッピング
        builder.ToTable("orders");
        
        builder.Property(x => x.OrderNumber)
            .HasMaxLength(20) //文字の最大桁数
            .IsRequired();  //必須制約

        builder.HasIndex(x => x.OrderNumber) //OrderNumberで一意
            .IsUnique();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2); //数値の精度

        builder.HasOne(x => x.Customer) // 1 対 多 (Customer → Order) CustomerId が 外部キー
            .WithMany(x => x.Orders)  
            .HasForeignKey(x => x.CustomerId);
    }
}