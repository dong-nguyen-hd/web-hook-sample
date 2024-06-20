﻿namespace WebHookSample.Domain.Context.Config;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Header
/// </summary>
public sealed class HeaderConfig : IEntityTypeConfiguration<Models.Header>
{
    public void Configure(EntityTypeBuilder<Models.Header> entity)
    {
        entity.ToTable("tbl_header");

        entity.Property(x => x.Id).UseIdentityColumn();
        entity.HasKey(x => new { x.Id, x.Uuid });
    }
}
