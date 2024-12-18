using AutoMapper;
using TodoProject.Models;
using TodoProject.ModelsDTO;

namespace TodoProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodoController(TodoAppContext context, IMapper mapper) : ControllerBase
    {
        private readonly TodoAppContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet("get-todos")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var todos = await _context.Todos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TodoDTO
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    DateTask = x.DateTask,
                    IsCompleted = x.IsCompleted
                }).ToListAsync();

                int totalCount = await _context.Todos.CountAsync();

                return Ok(new
                {
                    Data = todos,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла непредвиденная ошибка: {ex.Message}. Пожалуйста, попробуйте позже");
            }
        }

        [HttpPost("create-todo")]
        public async Task<IActionResult> Create(TodoDTO todoDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var todo = _mapper.Map<Todo>(todoDTO);
                    _context.Todos.Add(todo);
                    await _context.SaveChangesAsync();
                    return Ok("Данные успешно добавлены");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла непредвиденная ошибка: {ex.Message}. Пожалуйста, попробуйте позже");
            }
        }

        [HttpPut("update-todo")]
        public async Task<IActionResult> Update(TodoDTO todoDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!await _context.Todos.AnyAsync(x => x.Id == todoDTO.Id))
                        return BadRequest($"Запись с Id {todoDTO.Id} не найдена");

                    var todo = _mapper.Map<Todo>(todoDTO);
                    _context.Todos.Update(todo);
                    await _context.SaveChangesAsync();
                    return Ok("Данные успешно обновлены");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла непредвиденная ошибка: {ex.Message}. Пожалуйста, попробуйте позже");
            }
        }

        [HttpDelete("delete-todo")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                    return BadRequest($"Запись с Id {id} не найдена");

                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
                return Ok("Данные успешно удалены");
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла непредвиденная ошибка: {ex.Message}. Пожалуйста, попробуйте позже");
            }
        }
    }
}
