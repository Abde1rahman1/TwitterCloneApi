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
	public class CommentsController : ControllerBase
	{

		private readonly ApplicationDbContext _context;

		public CommentsController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost("{PostId}")]
		[Authorize]
		public async Task<IActionResult> AddNewComment(int PostId, [FromBody] AddCommentDto commentDto)
		{
			var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == PostId);
			if (post == null)
			{
				return NotFound();
			}
			// Get the current user who is adding the comment
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
			if (user == null) 
			{
				return Unauthorized();
			}

			var newComment = new Comment
			{
				Content = commentDto.Content,
				PostId= PostId,
				ApplicationUserId = currentUserId,
				Username= user.UserName,
			};

			 _context.Comments.Add(newComment);
			await _context.SaveChangesAsync();

			return Ok(newComment);
		}

		[HttpPut("{CommentId}")]
		[Authorize]
		public async Task<IActionResult> EditComment(int CommentId, AddCommentDto commentDto)
		{
			var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == CommentId);
			if (comment == null) { return NotFound(); }

			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (comment.ApplicationUserId != currentUserId)
			{
				return Unauthorized("You are not authorized to edit this comment.");
			}

			comment.Content = commentDto.Content;
			_context.Comments.Update(comment);
			await _context.SaveChangesAsync();
			return Ok(comment);
		}

		[HttpDelete("{CommentId}")]
		[Authorize]
		public async Task<IActionResult> DeleteComment(int CommentId)
		{
			var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == CommentId);
			if (comment == null) { return NotFound(); }

			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// check if the user is the owner to this comment 
			if (comment.ApplicationUserId != currentUserId)
			{
				return Unauthorized("You are not authorized to delete this comment.");
			}

			_context.Comments.Remove(comment);
			await _context.SaveChangesAsync();
			return Ok(comment);
		}

		[HttpGet("{PostId}")]
		public async Task<IActionResult> GetCommentsByPostId(int PostId)
		{
			var post = await _context.Comments.FirstOrDefaultAsync(p=>p.PostId == PostId);
			if(post == null) { return NotFound(); }
			var posts = await _context.Comments
				.Where(c => c.PostId == PostId)
				.Select(c=>new Comment
				{ 
					Content=c.Content,
					Username=c.ApplicationUser.UserName
				})
				.ToListAsync();

			return Ok(posts);
		}
	}
}
