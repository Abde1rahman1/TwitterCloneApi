using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TwitterCloneApi.Models
{
	public class Comment
	{
        public int CommentId { get; set; }

		public string Content { get; set; }

		[ForeignKey("Post")]
		public int PostId { get; set; }

		[JsonIgnore]
		public Post Post { get; set; }


		[ForeignKey("ApplicationUser")]
		public string ApplicationUserId { get; set; }

		[JsonIgnore]
		public ApplicationUser ApplicationUser { get; set; }

        public string Username { get; set; }
    }
}
