using CompRoomAuthApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompRoomAuthApi.Contexts
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
		{
		}

		public DbSet<Room> Rooms { get; set; }
		public DbSet<Computer> Computers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Room and Computer relationship
			modelBuilder.Entity<Room>()
				.HasOne(r => r.User)
				.WithOne()
				.HasForeignKey<Room>(r => r.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Computer>()
				.HasOne(r => r.Room)
				.WithMany()
				.HasForeignKey(r => r.RoomId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
