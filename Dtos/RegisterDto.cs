namespace LoginApp.Dtos
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string AdminCode { get; set; }
    }
}