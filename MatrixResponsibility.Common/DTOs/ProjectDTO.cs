using System;
using System.Collections.Generic;

namespace MatrixResponsibility.Common.DTOs
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? AB { get; set; }
        public string? InternalMeeting { get; set; }
        public string? ReportStatus { get; set; }
        public string? Customer { get; set; }
        public string? StartPermissionLetter { get; set; }
        public string? MarketingName { get; set; }
        public string? ObjectAddress { get; set; }
        public DateTime? GPZUDate { get; set; }
        public DateTime? DateStartPD { get; set; }
        public DateTime? DateFirstApproval { get; set; }
        public DateTime? DateStartRD { get; set; }
        public DateTime? DateEndRD { get; set; }
        public double? TotalArea { get; set; }
        public double? SaleableArea { get; set; }
        public UserDTO GIP { get; set; } = null!;
        public UserDTO AssistantGIP { get; set; } = null!;
        public UserDTO GAP { get; set; } = null!;
        public UserDTO GKP { get; set; } = null!;
        public UserDTO GP { get; set; } = null!;
        public UserDTO EOM { get; set; } = null!;
        public UserDTO SS { get; set; } = null!;
        public UserDTO AK { get; set; } = null!;
        public UserDTO Responsible { get; set; } = null!;
        public BKPDTO BKP { get; set; } = null!;
        public List<ProjectCorrectionDTO> Corrections { get; set; } = [];
        public ProjectDTO()
        {

        }
    }
}
