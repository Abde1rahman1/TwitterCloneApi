using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserFollowsController : ControllerBase
	{

		private readonly ApplicationDbContext _context;

        public UserFollowsController(ApplicationDbContext context)
        {
            _context = context;
        }


		[HttpPost("{followedUsername}")]
		[Authorize]
		public async Task<IActionResult> FollowUser(string followedUsername)
		{

			var currentUsername = User.Identity.Name;

			var followerUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUsername);
			if (followerUser == null)
			{
				return Unauthorized("You are not authorized to perform this action.");
			}

			var followedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == followedUsername);
			if (followedUser == null)
			{
				return NotFound($"User '{followedUsername}' not found.");
			}

			if (followerUser.UserName == followedUser.UserName)
			{
				return BadRequest("You cannot follow yourself.");
			}

			var existingFollow = await _context.UserFollow
				.FirstOrDefaultAsync(uf => uf.FollowerUserId == followerUser.Id && uf.FollowedUserId == followedUser.Id);

			if (existingFollow != null)
			{
				return BadRequest("You are already following this user.");
			}

			var userFollow = new UserFollow
			{
				FollowerUserId = followerUser.Id,
				FollowedUserId = followedUser.Id,
			};

			_context.UserFollow.Add(userFollow);
			await _context.SaveChangesAsync();

			return Ok(userFollow);
		}

		[HttpDelete("{followedUsername}")]
		[Authorize]
		public async Task<IActionResult>DeleteFollow(string followedUsername)
		{
			var currentUsername = User.Identity.Name;
			var followerUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUsername);
			if (followerUser == null)
			{
				return Unauthorized("You are not authorized to perform this action.");
			}
			var followedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == followedUsername);
			if (followedUser == null)
			{
				return NotFound($"User '{followedUsername}' not found.");
			}

			if (followerUser.UserName == followedUser.UserName)
			{
				return BadRequest("You cannot delete follow  from yourself.");
			}

			var existingUser = await _context.UserFollow
				.FirstOrDefaultAsync(u=>u.FollowerUserId==followerUser.Id && u.FollowedUserId==followedUser.Id);

			if (existingUser == null)
			{
				return BadRequest("You are already don't follow this user.");
			}
			_context.UserFollow.Remove(existingUser);
			_context.SaveChanges();
			return Ok(existingUser);
		}

		[HttpGet("{Username}")]
		[Authorize]
		public async Task<IActionResult> GetAllFollowers(string Username)
		{
			var followedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == Username);
			if (followedUser == null)
			{
				return NotFound($"User '{Username}' not found.");
			}

			var followerList = await _context.UserFollow
				.Where(u => u.FollowedUserId == followedUser.Id)
				.Select(u => new {
					Username = u.FollowerUser.UserName
				})
				.ToListAsync();

			return Ok(followerList);
		}

		[HttpGet("Following/{FollowedUsername}")]
		[Authorize]
		public async Task<IActionResult> GetAllFollowing(string FollowedUsername)
		{
			var followerUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == FollowedUsername);
			if (followerUser == null)
			{
				return NotFound($"User '{FollowedUsername}' not found.");
			}

			var followedList = await _context.UserFollow
				.Where(u => u.FollowerUserId == followerUser.Id)
				.Select(u => new {
					Username = u.FollowedUser.UserName
				})
				.ToListAsync();

			return Ok(followedList);
		}

	}
}
