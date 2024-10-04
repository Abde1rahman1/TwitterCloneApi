using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TwitterCloneApi.Models
{
	public class Post
	{
		public int PostId {  get; set; }

        public string PostContent { get; set; }

		[ForeignKey("ApplicationUser")]
		public string ApplicationUserId { get; set; }
		[JsonIgnore]
		public ApplicationUser ApplicationUser { get; set; }

		public ICollection<Like> Likes { get; set; }
		public ICollection<Comment> Comments { get; set; }


	}
}
