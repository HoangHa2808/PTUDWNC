﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Mappings;

namespace TatBlog.Data.Contexts;

public class BlogDbContext : DbContext
{
	public DbSet<Author> Authors { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Post> Posts { get; set; }
	public DbSet<Tag> Tags { get; set; }

	protected override void OnConfiguring(
		DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlServer(@"Server=localhost;Database=TatBlog;
Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(CategoryMap).Assembly);
	}
}
