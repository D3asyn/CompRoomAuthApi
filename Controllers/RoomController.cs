using CompRoomAuthApi.Contexts;
using CompRoomAuthApi.DTOs;
using CompRoomAuthApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CompRoomAuthApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RoomController(ApplicationDbContext context) : ControllerBase
	{
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Post([FromBody] PostRoom roomDto)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier);


			if(roomDto is null)
			{
				return BadRequest("Empty input");
			}

			var room = new Room
			{
				Name = roomDto.Name,
				Capacity = roomDto.Capacity,
				UserId = userId!.Value
			};

			await context.Rooms.AddAsync(room);
			await context.SaveChangesAsync();
			return Ok(room);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Get()
		{
			var rooms = await context.Rooms.Include(r => r.User).ToListAsync();

			if(rooms is null)
			{
				return NotFound("No rooms found");
			}

			var result = rooms.Select(r => new GetRoom
			{
				Name = r.Name,
				Capacity = r.Capacity,
				UserName = r.User.UserName!
			});

			return Ok(result);
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> Put(int id, [FromBody] PutRoom roomDto)
		{
			var room = await context.Rooms.SingleOrDefaultAsync(r => r.Id == id);
			if(room == null)
			{
				return NotFound("Room not found");
			}

			room.Name = roomDto.Name;
			room.Capacity = roomDto.Capacity;

			await context.SaveChangesAsync();
			return Ok("Updated successfuly");
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var room = await context.Rooms.SingleOrDefaultAsync(r => r.Id == id);
			if(room == null)
			{
				return NotFound("Room found");
			}

			context.Rooms.Remove(room);
			await context.SaveChangesAsync();
			return Ok("Deleted successfuly");
		}
	}
}
