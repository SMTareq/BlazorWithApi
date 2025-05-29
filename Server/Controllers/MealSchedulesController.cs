using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorWithApi.Shared.Models;
using BlazorWithApi.Server.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Net.Mime;

namespace BlazorWithApi.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class MealSchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MealSchedulesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/MealSchedules
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MealScheduleDto>>> GetMealSchedules()
        {
            var mealSchedules = await _context.MealSchedules
                .Include(ms => ms.Employee)
                .ProjectTo<MealScheduleDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(mealSchedules);
        }

        // GET: api/MealSchedules/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealScheduleDto>> GetMealSchedule(int id)
        {
            var mealSchedule = await _context.MealSchedules
                .Include(ms => ms.Employee)
                .FirstOrDefaultAsync(ms => ms.ScheduleId == id);

            if (mealSchedule == null)
            {
                return NotFound();
            }

            return _mapper.Map<MealScheduleDto>(mealSchedule);
        }

        // GET: api/MealSchedules/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<MealScheduleDto>>> GetMealSchedulesByEmployee(int employeeId)
        {
            return await _context.MealSchedules
                .Where(ms => ms.EmployeeId == employeeId)
                .ProjectTo<MealScheduleDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/MealSchedules/date/2023-01-01
        [HttpGet("date/{date:datetime}")]
        public async Task<ActionResult<IEnumerable<MealScheduleDto>>> GetMealSchedulesByDate(DateTime date)
        {
            return await _context.MealSchedules
                .Where(ms => ms.MealDate.HasValue && ms.MealDate.Value.Date == date.Date)
                .Include(ms => ms.Employee)
                .ProjectTo<MealScheduleDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // POST: api/MealSchedules/batch
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostMealSchedulesBatch(List<MealScheduleDto> mealSchedulesDto)
        {
            if (mealSchedulesDto == null || !mealSchedulesDto.Any())
            {
                return BadRequest("No meal schedules provided");
            }

            // Get the date from the first schedule, ensuring it's not null
            var firstSchedule = mealSchedulesDto.FirstOrDefault();
            if (firstSchedule == null)
            {
                return BadRequest("Invalid meal schedule data");
            }
            
            // Ensure MealDate is not null before accessing Date property
            if (!firstSchedule.MealDate.HasValue)
            {
                return BadRequest("Meal date is required");
            }
            var date = firstSchedule.MealDate.Value.Date;
            
            // Get existing schedules for the date
            var existingSchedules = await _context.MealSchedules
                .Where(ms => ms.MealDate.HasValue && ms.MealDate.Value.Date == date)
                .ToListAsync();

            // Remove existing schedules for the date
            if (existingSchedules.Any())
            {
                _context.MealSchedules.RemoveRange(existingSchedules);
                await _context.SaveChangesAsync();
            }

            // Add new schedules
            var newSchedules = _mapper.Map<List<MealSchedule>>(mealSchedulesDto);
            
            _context.MealSchedules.AddRange(newSchedules);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/MealSchedules/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutMealSchedule(int id, MealScheduleDto mealScheduleDto)
        {
            if (id != mealScheduleDto.ScheduleId)
            {
                return BadRequest("ID in the URL does not match the ID in the request body.");
            }

            var existingSchedule = await _context.MealSchedules.FindAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            // Map the DTO to the entity
            _mapper.Map(mealScheduleDto, existingSchedule);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!MealScheduleExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/MealSchedules
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MealScheduleDto>> PostMealSchedule(MealScheduleDto mealScheduleDto)
        {
            var mealSchedule = _mapper.Map<MealSchedule>(mealScheduleDto);
            
            _context.MealSchedules.Add(mealSchedule);
            await _context.SaveChangesAsync();

            // Map back to DTO to include any database-generated values
            var resultDto = _mapper.Map<MealScheduleDto>(mealSchedule);
            
            return CreatedAtAction(
                nameof(GetMealSchedule), 
                new { id = mealSchedule.ScheduleId }, 
                resultDto);
        }

        // DELETE: api/MealSchedules/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMealSchedule(int id)
        {
            var mealSchedule = await _context.MealSchedules.FindAsync(id);
            if (mealSchedule == null)
            {
                return NotFound();
            }

            _context.MealSchedules.Remove(mealSchedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MealScheduleExists(int id)
        {
            return _context.MealSchedules.Any(e => e.ScheduleId == id);
        }
    }
}
