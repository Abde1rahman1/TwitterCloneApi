using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TwitterCloneApi.Dtos;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public PostsController(ApplicationDbContext context) { _context = context; }


		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAllPosts()
		{
			var posts = await _context.Posts.Include(l => l.Likes)
				.Include(c => c.Comments)
				.Include(u => u.ApplicationUser)
				.Select(p => new PostDto
				{
					PostId = p.PostId,
					Likes = p.Likes,
					Comments = p.Comments,
					PostContent = p.PostContent,
				
					Username = p.ApplicationUser.UserName

				}).ToListAsync();

			return Ok(posts);

		}

		[HttpGet("{Username}")]
		//[Authorize]
		public async Task<IActionResult> GetPostById(string Username)
		{
			var posts = await _context.Posts.Include(l => l.Likes)
				.Include(c => c.Comments)
				.Include(u => u.ApplicationUser)
				.Where(p => p.ApplicationUser.UserName == Username)
				.Select(p => new PostDto
				{
					PostId = p.PostId,
					Likes = p.Likes,
					Comments = p.Comments,
					PostContent = p.PostContent,
				
					Username = p.ApplicationUser.UserName

				})
				.ToListAsync();

			return Ok(posts);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreatePost([FromBody] AddPostDto dto)
		{
			// Check if the data is valid
			if (dto == null || dto.PostContent.Length > 500)
			{
				return BadRequest("Invalid Data");
			}

			// Get the current logged-in user

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
			{
				return Unauthorized("User not found");
			}

			// Create a new Post
			var newPost = new Post
			{
				PostContent = dto.PostContent,
				ApplicationUserId = userId,
				Likes = new List<Like>(),
				Comments = new List<Comment>()
			};

			// Add the post to the database
			_context.Posts.Add(newPost);
			await _context.SaveChangesAsync();

			var createdPost = await _context.Posts
											  .Include(p => p.ApplicationUser) // Include the related ApplicationUser entity
											  .FirstOrDefaultAsync(p => p.PostId == newPost.PostId);
			return Ok(newPost);
		}


		[HttpPut("{PostId}")]
		[Authorize]
		public async Task<IActionResult> UpdatePostById(int PostId, [FromBody] AddPostDto dto)
		{
			var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == PostId);

			if (post == null)
			{
				return NotFound();
			}

			// check if user is the owner of post
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (post.ApplicationUserId != currentUserId)
			{
				return Forbid();
			}

			post.PostContent = dto.PostContent;

			_context.Posts.Update(post);
			_context.SaveChanges();
			return Ok(post);
		}

		[HttpDelete("{PostId}")]
		[Authorize]
		public async Task <IActionResult> DeleteById (int PostId)
		{
			var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == PostId);

			if (post == null)
			{
				return NotFound();
			}

			// check if user is the owner of post
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (post.ApplicationUserId != currentUserId)
			{
				return Forbid();
			}

			_context.Posts.Remove(post);
			_context.SaveChanges();

			return Ok(post);
		}
	}
}
