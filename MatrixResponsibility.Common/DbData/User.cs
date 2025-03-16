using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrixResponsibility.Common
{
    /// <summary>
    /// Модель пользователя.
    /// </summary>
    [Table("users")]
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Логин пользователя.
        /// </summary>
        [Column("login")]
        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// ФИО пользователя.
        /// </summary>
        [Column("fio")]
        [MaxLength(100)]
        public string FIO { get; set; } = string.Empty;


        /// <summary>
        /// Email пользователя.
        /// </summary>
        [Column("email")]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        /// <summary>
        /// Коллекция ролей, связанных с пользователем.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }
    }
}