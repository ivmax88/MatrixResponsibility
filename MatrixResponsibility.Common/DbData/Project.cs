using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrixResponsibility.Common
{
    /// <summary>
    /// Модель проекта.
    /// </summary>
    [Table("projects")]
    public class Project
    {
        /// <summary>
        /// Уникальный идентификатор проекта.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Название объекта.
        /// </summary>
        [Column("project_name")]
        [MaxLength(255)]
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор главного инженера проекта.
        /// </summary>
        [Column("gip_id")]
        public int? GIPId { get; set; }

        /// <summary>
        /// Идентификатор помощника главного инженера проекта.
        /// </summary>
        [Column("assistant_gip_id")]
        public int? AssistantGIPId { get; set; }

        /// <summary>
        /// Идентификатор главного архитектора проекта.
        /// </summary>
        [Column("gap_id")]
        public int? GAPId { get; set; }

        /// <summary>
        /// Идентификатор главного конструктора проекта.
        /// </summary>
        [Column("gkp_id")]
        public int? GKPId { get; set; }

        /// <summary>
        /// Архитектурное бюро.
        /// </summary>
        [Column("ab")]
        [MaxLength(100)]
        public string? AB { get; set; }

        /// <summary>
        /// Идентификатор генерального проектировщика.
        /// </summary>
        [Column("gp_id")]
        public int? GPId { get; set; }

        /// <summary>
        /// Идентификатор ответственного за электрооборудование и механику.
        /// </summary>
        [Column("eom_id")]
        public int? EOMId { get; set; }

        /// <summary>
        /// Идентификатор ответственного за слаботочные системы.
        /// </summary>
        [Column("ss_id")]
        public int? SSId { get; set; }

        /// <summary>
        /// Идентификатор ответственного за автоматизацию и контроль.
        /// </summary>
        [Column("ak_id")]
        public int? AKId { get; set; }

        /// <summary>
        /// Идентификатор ответственного лица за объект.
        /// </summary>
        [Column("responsible_id")]
        public int? ResponsibleId { get; set; }

        /// <summary>
        /// Информация о внутреннем совещании.
        /// </summary>
        [Column("internal_meeting")]
        [MaxLength(100)]
        public string? InternalMeeting { get; set; }

        /// <summary>
        /// Статус отчета.
        /// </summary>
        [Column("report_status")]
        [MaxLength(50)]
        public string? ReportStatus { get; set; }

        /// <summary>
        /// Дата градостроительного плана земельного участка.
        /// </summary>
        [Column("gpzu_date")]
        public DateTime? GPZUDate { get; set; }

        /// <summary>
        /// Заказчик проекта.
        /// </summary>
        [Column("customer")]
        [MaxLength(100)]
        public string? Customer { get; set; }

        /// <summary>
        /// Письмо о разрешении старта проектной документации или рабочей документации.
        /// </summary>
        [Column("start_permission_letter")]
        [MaxLength(255)]
        public string? StartPermissionLetter { get; set; }

        /// <summary>
        /// Маркетинговое название объекта.
        /// </summary>
        [Column("marketing_name")]
        [MaxLength(255)]
        public string? MarketingName { get; set; }

        /// <summary>
        /// Адрес объекта.
        /// </summary>
        [Column("object_address")]
        [MaxLength(1000)]
        public string? ObjectAddress { get; set; }

        /// <summary>
        /// Идентификатор Бюро комплексного проектирования.
        /// </summary>
        [Column("bkp_id")]
        public int? BKPId { get; set; }

        /// <summary>
        /// Дата начала проектирования ПД.
        /// </summary>
        [Column("date_start_pd")]
        public DateTime? DateStartPD { get; set; }

        /// <summary>
        /// Дата положительного заключения первичного.
        /// </summary>
        [Column("date_first_approval")]
        public DateTime? DateFirstApproval { get; set; }

        /// <summary>
        /// Дата начала проектирования РД.
        /// </summary>
        [Column("date_start_rd")]
        public DateTime? DateStartRD { get; set; }

        /// <summary>
        /// Дата окончания проектирования РД.
        /// </summary>
        [Column("date_end_rd")]
        public DateTime? DateEndRD { get; set; }

        /// <summary>
        /// Наименование проекта.
        /// </summary>
        [Column("name")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Общая площадь.
        /// </summary>
        [Column("total_area")]
        public double? TotalArea { get; set; }

        /// <summary>
        /// Продаваемая площадь.
        /// </summary>
        [Column("saleable_area")]
        public double? SaleableArea { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с главным инженером проекта.
        /// </summary>
        public User? GIP { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с помощником главного инженера проекта.
        /// </summary>
        public User? AssistantGIP { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с главным архитектором проекта.
        /// </summary>
        public User? GAP { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с главным конструктором проекта.
        /// </summary>
        public User? GKP { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с генпланом.
        /// </summary>
        public User? GP { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с ответственным за электрооборудование и механику.
        /// </summary>
        public User? EOM { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с ответственным за слаботочные системы.
        /// </summary>
        public User? SS { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с ответственным за автоматизацию и контроль.
        /// </summary>
        public User? AK { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с ответственным лицом.
        /// </summary>
        public User? Responsible { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Бюро комплексного проектирования.
        /// </summary>
        public BKP? BKP { get; set; }

        /// <summary>
        /// Коллекция корректировок проекта.
        /// </summary>
        public ICollection<ProjectCorrection>? Corrections { get; set; }
    }
}