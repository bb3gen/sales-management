using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagement.Api.Domain.Entities;

namespace SalesManagement.Api.Features.Orders.Configurations;

public sealed class OrderDetailConfiguration
    : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        // マッピングする物理テーブル
        builder.ToTable("order_details");

        // 数値の精度設定
        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        // 数値の精度設定
        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        // 数値の精度設定
        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        // 1 対 多 OrderDetailは1つのOrderに属し、Orderは複数のOrderDetailを持つ OrderId列が外部キー
        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderDetails)  // .WithManyを省略すると 1 対 0/1 となる
            .HasForeignKey(x => x.OrderId);

        // 1 対 多 OrderDetailは1つのProductに属し、Productは複数のOrderDetailを持つ ProductId列が外部キー
        builder.HasOne(x => x.Product)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.ProductId);

        // OrderIdとLineNoで一意
        builder.HasIndex(x => new
        {
            x.OrderId,
            x.LineNo
        }).IsUnique();
    }
}