using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrixResponsibility.Common
{
    /// <summary>
    /// Модель Бюро комплексного проектирования (БКП).
    /// </summary>
    [Table("bkps")]
    public class BKP
    {
        /// <summary>
        /// Уникальный идентификатор Бюро комплексного проектирования.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Название Бюро комплексного проектирования.
        /// </summary>
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор директора Бюро комплексного проектирования.
        /// </summary>
        [Column("director_id")]
        public int DirectorId { get; set; }

        /// <summary>
        /// Навигационное свойство для директора.
        /// </summary>
        public User Director { get; set; } = null!;

        /// <summary>
        /// Коллекция проектов, связанных с этим Бюро.
        /// </summary>
        public ICollection<Project>? Projects { get; set; }
    }
}