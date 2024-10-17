namespace CompRoomAuthApi.DTOs
{
	public class PutComputer
	{
		public string Model { get; set; }
		public DateTime ManufacturedDate { get; set; }
		public int RoomId { get; set; }
	}
}
