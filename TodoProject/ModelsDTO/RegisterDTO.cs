namespace TodoProject.ModelsDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        public string? Password { get; set; }
    }
}
