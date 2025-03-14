using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrixResponsibility.Common
{
    /// <summary>
    /// Модель связи между пользователем и ролью.
    /// </summary>
    [Table("user_roles")]
    public class UserRole
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [Column("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// Идентификатор роли.
        /// </summary>
        [Column("role_id")]
        public int RoleId { get; set; }

        /// <summary>
        /// Навигационное свойство для пользователя.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Навигационное свойство для роли.
        /// </summary>
        public Role Role { get; set; }
    }
}