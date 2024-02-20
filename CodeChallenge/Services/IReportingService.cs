using CodeChallenge.Models;
using System;

namespace CodeChallenge.Services
{
    // I opted to make a new service for Reporting. My logic for this was that, while reporting is tied to Employee's and could be considered
    // an Employee Service, I thought it might be better to have a separate Reporting service in case there are more features involving reporting
    // down the line. Granted, Employee objects do have DirectReports tied to them and the ReportingService is using the EmployeeService right now.
    // There can definitely be an arugment made for ReportingStructure functionality to be in the EmployeeService class.
    public interface IReportingService
    {
        ReportingStructure GetReportingStructureById(String id);
    }
}
