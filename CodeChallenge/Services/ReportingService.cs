using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Services
{
    public class ReportingService : IReportingService
    {
        private readonly ILogger<ReportingService> _logger;
        private readonly IEmployeeService _employeeService;
        public ReportingService(ILogger<ReportingService> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Method that gets the reporting structure for a given employee id
        /// </summary>
        /// <param name="id">The employee id to obtain the reporting structure of</param>
        /// <returns></returns>
        public ReportingStructure GetReportingStructureById(String id)
        {
            // Check if the id is valid input
            if (!String.IsNullOrEmpty(id))
            {
                // Create the ReportingStructure that will eventually be returned
                ReportingStructure reportingStructure = new ReportingStructure();

                // Get the employee object via the already created method
                Employee employee = _employeeService.GetById(id);

                // Check if the employee exists, if it doesn't, no need to run through the rest of the logic
                if (employee != null)
                {
                    // The employee exists, so we want to set it to the ReportingStructure's Employee
                    reportingStructure.Employee = employee;


                    // Use the helper method to calculate the number of reports the employee has and set the field afterwards (even if there are no reports, we want to include 0)
                    int reports = CalculateReports(employee);
                    reportingStructure.NumberOfReports = reports;

                    return reportingStructure;
                }

            }
            return null;

        }

        #region helper-methods
        /// <summary>
        /// A helper method to recursively determine the number of reports a given employee has
        /// </summary>
        /// <param name="employee">The employee that will have their total number of reports calculated</param>
        /// <returns></returns>
        private int CalculateReports(Employee employee)
        {
            // Note: Do not cache report numbers; a/c states that values should be computed on the fly and should not persist            
            int reports = 0;

            // Validate that the employee actually has DirectReports and skip the logic if they do not
            if (employee.DirectReports != null)
            {
                // Add the number of DirectReports to our iterator so we track these, then get into the nested reports
                reports += employee.DirectReports.Count;

                // We need to check every DirectReport employee and see if they have any DirectReports and add them to our iterator if so
                foreach (var report in employee.DirectReports)
                {
                    // We will have the employee IDs from the DirectReports but we won't have the Employee object itself, so we need to actually get that
                    // Once we have that Employee, we can recursively call this method again to keep going with the count until there are no more DirectReports

                    // Validated that GetById will return null if the ID used is not in the system as an employeeID - D.F. 2/19/2024
                    reports += CalculateReports(_employeeService.GetById(report.EmployeeId));
                }
            }
            return reports;
        }
        #endregion
    }
}
