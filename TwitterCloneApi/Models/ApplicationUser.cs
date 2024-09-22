using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace TwitterCloneApi.Models
{
	public class ApplicationUser:IdentityUser
	{

		public string Bio { get; set; }
		public string Location { get; set; }
		public DateTime Birthday { get; set; }
		public DateTime JoinTime { get; set; }
		public string Gender { get; set; }

		[JsonIgnore]
		public List<Post> Posts { get; set; }
		= new List<Post>();
		
	}
}


// valid username : abdo 
// password : Abdo123@


// valid username : ahmed 
// password : ahmed123@