namespace TwitterCloneApi.Dtos
{
	public class RegisterDto
	{
		public string Username { get; set; }

		public string Password {  get; set; } 
		public string Bio { get; set; }
		public string Location { get; set; }
		public DateTime Birthday { get; set; }
		public DateTime JoinTime { get; set; }
		public string Gender { get; set; }
	}
}
