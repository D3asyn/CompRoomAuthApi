using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CompRoomAuthApi.Entities
{
	public class Computer
	{
		[Key]
		public int Id { get; set; }
		[MaxLength(255)]
		public string Model { get; set; }
		public DateTime ManufacturedDate { get; set; }
		public int RoomId { get; set; }

		[JsonIgnore]
		public virtual Room Room { get; set; }

	}
}
