using System;
using System.Collections.Generic;

namespace CodeChallenge.Models
{
    // Model for ReportingStructure that contains the Employee and NumberOfReports.
    // Note: The NumberOfReports is determined to be the number of directReports for an employee and all of their direct reports
    public class ReportingStructure
    {
        public Employee Employee { get; set; }
        public int NumberOfReports { get; set; }

    }
}
