using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings
{
	internal class CategoryMap : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.ToTable("Category");
			builder.HasKey(c=> c.Id);

			builder.Property(c=> c.Name)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(c=> c.Description)
				.HasMaxLength(500);

			builder.Property(c=> c.UrlSlug)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(c => c.ShowOnMenu)
				.IsRequired()
				.HasDefaultValue(false);

		}
	}
}
