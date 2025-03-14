using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrixResponsibility.Common
{
    /// <summary>
    /// Модель корректировки проектной документации.
    /// </summary>
    [Table("project_corrections")]
    public class ProjectCorrection
    {
        /// <summary>
        /// Уникальный идентификатор корректировки.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор проекта, к которому относится корректировка.
        /// </summary>
        [Column("project_id")]
        public int ProjectId { get; set; }

        /// <summary>
        /// Дата начала корректировки.
        /// </summary>
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата положительного заключения в ГК ОП.
        /// </summary>
        [Column("approval_date")]
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// Номер корректировки.
        /// </summary>
        [Column("correction_number")]
        public int CorrectionNumber { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с проектом.
        /// </summary>
        public Project Project { get; set; }
    }
}