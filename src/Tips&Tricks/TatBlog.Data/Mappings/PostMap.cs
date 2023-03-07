using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Datp.Mappings;

public class PostMap :IEntityTypeConfiguration<Post>
{
	public void Configure(EntityTypeBuilder<Post> builder)
	{
		builder.ToTable("Posts");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title)
			.HasMaxLength(500)
			.IsRequired();

		builder.Property(p => p.ShortDescription)
			.HasMaxLength(5000)
			.IsRequired();

		builder.Property(p => p.Description)
			.HasMaxLength(5000)
			.IsRequired();

		builder.Property(p => p.UrlSlug)
			.HasMaxLength(200)
			.IsRequired();

		builder.Property(p => p.Meta)
			.HasMaxLength(1000)
			.IsRequired();

		builder.Property(p => p.ImageUrl)
			.HasMaxLength(1000);

		builder.Property(p => p.ViewCount)
			.IsRequired()
			.HasDefaultValue(0);

		builder.Property(p => p.Published)
			.IsRequired()
			.HasDefaultValue(false);

		builder.Property(p => p.PostedDate)
			.HasColumnType("datetime");

		builder.Property(p => p.ModifiedDate)
			.HasColumnType("datetime");

		builder.HasOne(p => p.Category)
			.WithMany(c => c.Posts)
			.HasForeignKey(p => p.CategoryId)
			.HasConstraintName("FK_Posts_Categories")
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(p => p.Author)
			.WithMany(c => c.Posts)
			.HasForeignKey(p => p.AuthorId)
			.HasConstraintName("FK_Posts_Authors")
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(p => p.Tags)
			.WithMany(t => t.Posts)
			.UsingEntity(pt => pt.ToTable("PostTags"));

	}
}
