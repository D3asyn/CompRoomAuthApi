using CompRoomAuthApi.Contexts;
using CompRoomAuthApi.DTOs;
using CompRoomAuthApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompRoomAuthApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ComputerController(ApplicationDbContext context) : ControllerBase
	{
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Post([FromBody] PostComputer computerDto)
		{

			if(computerDto is null)
			{
				return BadRequest("Empty input");
			}

			var computer = new Computer
			{
				Model = computerDto.Model,
				ManufacturedDate = computerDto.ManufacturedDate,
				RoomId = computerDto.RoomId,
			};

			await context.Computers.AddAsync(computer);
			await context.SaveChangesAsync();
			return Ok(computer);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Get()
		{
			var computers = await context.Computers.Include(r => r.Room).ToListAsync();

			if(computers is null)
			{
				return NotFound("No computers found");
			}

			var result = computers.Select(c => new GetComputer
			{
				Model = c.Model,
				ManufactureDate = c.ManufacturedDate,
				RoomName = c.Room.Name!
			});

			return Ok(result);
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> Put(int id, [FromBody] PutComputer computerDto)
		{
			var computer = await context.Computers.SingleOrDefaultAsync(c => c.Id == id);
			if(computer == null)
			{
				return NotFound("Computer not found");
			}

			computer.Model = computerDto.Model;
			computer.ManufacturedDate = computerDto.ManufacturedDate;
			computer.RoomId = computerDto.RoomId;

			await context.SaveChangesAsync();
			return Ok("Updated successfuly");
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var computer = await context.Computers.SingleOrDefaultAsync(c => c.Id == id);
			if(computer == null)
			{
				return NotFound("Computer not found");
			}

			context.Computers.Remove(computer);
			await context.SaveChangesAsync();
			return Ok("Deleted successfuly");
		}
	}
}
