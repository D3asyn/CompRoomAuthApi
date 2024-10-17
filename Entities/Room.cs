using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CompRoomAuthApi.Entities
{
	public class Room
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(255)]
		public string Name { get; set; }
		public int Capacity { get; set; }
		// Foreign key for User
		public string? UserId { get; set; }
		[JsonIgnore]
		public virtual ApplicationUser User { get; set; }
	}
}
