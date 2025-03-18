using System;

namespace MatrixResponsibility.Common.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string FIO { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserDTO(User? user)
        {
            if (user == null) throw new NullReferenceException("user is null on UserDTO creating");
            Id = user.Id;
            Login = user.Login;
            FIO = user.FIO;
            Email = user.Email;
        }
        public UserDTO()
        {
            
        }
    }
}
