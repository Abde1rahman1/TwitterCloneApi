using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace TwitterCloneApi.Models
{
	public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			
		}
		public DbSet<Post> Posts { get; set; }
		public DbSet <Comment>Comments { get; set; }	

		public DbSet<Like> Likes { get; set; }
		public DbSet<UserFollow> UserFollow { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configure the relationship between Post and Like
			builder.Entity<Like>()
				.HasOne(l => l.Post)
				.WithMany(p => p.Likes)
				.HasForeignKey(l => l.PostId)
				.OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

			// Similarly, you may need to configure other relationships if required
			builder.Entity<Comment>()
				.HasOne(c => c.Post)
				.WithMany(p => p.Comments)
				.HasForeignKey(c => c.PostId)
				.OnDelete(DeleteBehavior.Restrict); // Disable cascading delete for Comments if needed

			builder.Entity<Like>()
				.HasOne(l => l.Post)
				.WithMany(p => p.Likes)
				.HasForeignKey(l => l.PostId)
				.OnDelete(DeleteBehavior.Restrict); // Disable cascade delete for this relationship

			// Keep cascade delete for Likes -> ApplicationUser relationship

			builder.Entity<UserFollow>(entity =>
			{
				entity.HasKey(uf => uf.Id); // Primary key

				// Configuring FollowerUser relationship (User who follows)
				entity
					.HasOne(uf => uf.FollowerUser)        // Navigation property
					.WithMany()                           // No reverse navigation needed here
					.HasForeignKey(uf => uf.FollowerUserId) // Foreign key
					.OnDelete(DeleteBehavior.Restrict);   // Restrict or cascade as needed

				// Configuring FollowedUser relationship (User being followed)
				entity
					.HasOne(uf => uf.FollowedUser)        // Navigation property
					.WithMany()                           // No reverse navigation needed here
					.HasForeignKey(uf => uf.FollowedUserId) // Foreign key
					.OnDelete(DeleteBehavior.Restrict);   // Restrict or cascade as needed

				// Optional: Add Unique Constraint to prevent duplicate follows (same user following same user twice)
				entity.HasIndex(uf => new { uf.FollowerUserId, uf.FollowedUserId })
					  .IsUnique();
			});
		}

	}
}
