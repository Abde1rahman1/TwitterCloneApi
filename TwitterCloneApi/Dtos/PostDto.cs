using TwitterCloneApi.Models;

namespace TwitterCloneApi.Dtos
{
	public class PostDto
	{
		public int PostId { get; set; }

		public string PostContent { get; set; }
		public ICollection<Like> Likes { get; set; }
		public ICollection<Comment> Comments { get; set; }

		public int LikeCounts { get; set; } = 0;
		public string Username { get; set; }

	}
}
