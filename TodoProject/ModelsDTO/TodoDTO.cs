namespace TodoProject.ModelsDTO
{
    public class TodoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок обязателен")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Описание обязателен")]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateTask { get; set; }
    }
}
