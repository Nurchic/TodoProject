using AutoMapper;
using Azure.Core;
using TodoProject.Models;
using TodoProject.ModelsDTO;
using TodoProject.Services;

namespace TodoProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(TodoAppContext context, TokenProvider token, IMapper mapper) : ControllerBase
    {
        private readonly TodoAppContext _context = context;
        private readonly TokenProvider _tokenProvider = token;
        private readonly IMapper _mapper = mapper;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _context.Users.AnyAsync(x => x.Username == registerDTO.Username))
                        return BadRequest("Пользователь уже существует");

                    var user = _mapper.Map<User>(registerDTO);
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return Ok("Пользователь успешно зарегистрирован");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла ошибка: {ex.Message} при регистрации пользователя. Пожалуйста, попробуйте позже");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(RegisterDTO registerDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerDTO.Username);
                    if (user == null || !BCrypt.Net.BCrypt.Verify(registerDTO.Password, user.PasswordHash))
                        return Unauthorized("Неправильное имя пользователя или пароль");

                    var accessToken = _tokenProvider.GenerateJwtToken(user);
                    var refreshToken = _tokenProvider.GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    });
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла непредвиденная ошибка: {ex.Message}. Пожалуйста, попробуйте позже");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                    if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                        return Unauthorized("Недействительный или истекший Refresh Token");

                    var newAccessToken = _tokenProvider.GenerateJwtToken(user);
                    var newRefreshToken = _tokenProvider.GenerateRefreshToken();

                    user.RefreshToken = newRefreshToken;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken
                    });
                }
                return BadRequest("Refresh Token не был передан");
            }
            catch (Exception ex)
            {
                return BadRequest($"Произошла непредвиденная ошибка: {ex.Message}. Пожалуйста, попробуйте позже");
            }
        }
    }
}
