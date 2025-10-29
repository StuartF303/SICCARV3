// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace Siccar.Application.Models
{
    public class UploadResult
    {
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public string? ErrorReason { get; set; }
    }
}
