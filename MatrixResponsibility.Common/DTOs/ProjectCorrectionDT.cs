using System;

namespace MatrixResponsibility.Common.DTOs
{
    public class ProjectCorrectionDTO
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public int CorrectionNumber { get; set; }
    }
}
