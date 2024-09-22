using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TwitterCloneApi.Models
{
	public class UserFollow
	{
		public int Id { get; set; }


		[ForeignKey("FollowerUser")]
		public string FollowerUserId { get; set; }

		[JsonIgnore]
		public ApplicationUser FollowerUser { get; set; }

		[ForeignKey("FollowedUser")]
		public string FollowedUserId { get; set; }
		[JsonIgnore]
		public ApplicationUser FollowedUser { get; set; }
		

	}
}
