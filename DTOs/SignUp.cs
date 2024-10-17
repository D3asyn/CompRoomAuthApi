using System.ComponentModel.DataAnnotations;

namespace CompRoomAuthApi.DTOs
{
	public class SignUp
	{
		[Required]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
