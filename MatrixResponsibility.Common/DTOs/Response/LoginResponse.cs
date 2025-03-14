using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MatrixResponsibility.Common.DTOs.Response
{
    public record LoginResponse(bool Success, string? Token);
}
