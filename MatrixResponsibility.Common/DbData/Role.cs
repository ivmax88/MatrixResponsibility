using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrixResponsibility.Common
{
    /// <summary>
    /// Модель роли пользователя.
    /// </summary>
    [Table("roles")]
    public class Role
    {
        /// <summary>
        /// Уникальный идентификатор роли.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Название роли.
        /// </summary>
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание роли.
        /// </summary>
        [Column("description")]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Коллекция пользователей, связанных с этой ролью.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }
    }
}