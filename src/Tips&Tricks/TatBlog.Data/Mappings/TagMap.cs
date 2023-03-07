using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;


namespace TatBlog.Datt.Mappings;

public class TagMap : IEntityTypeConfiguration<Tag>
{
	public void Configure(EntityTypeBuilder<Tag> builder)
	{
		builder.ToTable("Tags");

		builder.HasKey(t => t.Id);

		builder.Property(t => t.Name)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(t => t.Description)
			.HasMaxLength(500);

		builder.Property(t => t.UrlSlug)
			.HasMaxLength(50)
			.IsRequired();

	}
}
