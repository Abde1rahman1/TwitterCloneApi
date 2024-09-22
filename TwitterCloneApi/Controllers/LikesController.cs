using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LikesController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public LikesController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost("{PostId}")]
		[Authorize]
		public async Task<IActionResult> AddLike(int PostId)
		{
			var post = await _context.Posts.FirstOrDefaultAsync(p=>p.PostId == PostId);
			if (post == null) {return NotFound();}

			var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUser);
			if (user == null) {return Unauthorized(); }


			// Check if the user has already liked the post
			var existingLike = await _context.Likes
				.FirstOrDefaultAsync(l => l.PostId == PostId && l.ApplicationUserId == currentUser);

			if (existingLike != null)
			{
				return BadRequest("You have already liked this post.");
			}


			var like = new Like
			{
				
				PostId = PostId,
				ApplicationUserId=currentUser,
				Username=user.UserName
			};
			
			_context.Likes.Add(like);
			await _context.SaveChangesAsync();

			return Ok(like);
		}

		[HttpDelete("{LikeId}")]
		[Authorize]
		public async Task<IActionResult> DeleteLike(int LikeId)
		{
			var like = await _context.Likes.FirstOrDefaultAsync(l=>l.LikeId== LikeId);
			if (like == null) {return NotFound();}

			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// check if the user is the owner to this Like 
			if (like.ApplicationUserId != currentUserId)
			{
				return Unauthorized("You are not authorized to Delete this like.");
			}

			_context.Likes.Remove(like);
			_context.SaveChanges();
			return Ok(like);

		}

	}
}
