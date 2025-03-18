namespace MatrixResponsibility.Common.DTOs
{
    public class BKPDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public UserDTO Director { get; set; } = null!;
    }
}
