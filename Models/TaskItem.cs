namespace BackendApi.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public string? ImageUrl { get; set; }

        public string Username { get; set; } = string.Empty;
    }
}