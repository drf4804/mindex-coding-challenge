using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        // I'm not entirely sure why, but the original LINQ query was giving me trouble and I wasn't seeing DirectReports in the response when I should
        // have been. I made some changes and decided to use Include() because I was having issues getting the DirectReports to load in my response.
        // It should be noted that this ONLY works for double nested records, anything past that will still show up null.
        // There are potential solutions via recursion or Lazy Loading, but since there are no records that are more than double nested, this solution
        // works for the time being.
        // I also added the "virtual" tag to the Employee class/model just in case.
        // In addition, I looked into Lazy Loading but didn't want to install a new package (not sure if I was supposed to) so I decided to go with this approach
        // and get back to work on the tasks laid out in the challenge itself (I'm assuming this isn't intended as part of the challenge, but hey it might be!).

        // I also want to note that I'm not sure if this is the intended functionality, because the output will include entire Employee objects in the response now
        // and by looking at the EmployeeSeedData.json file it looks like the expected / desired functionality is to just have the EmployeeID be in the output.
        // Since I'm unsure of the a/c, I'm going to leave it this way since the Employee object defines DirectReports as List<Employee>, however if only the Id is
        // desired for DirectReports, a simple DirectReport model/class could be made and the Employee model could be
        // refactored to have a list of <DirectReport>DirectReports.
        // - D.F. 2/19/2024
        public Employee GetById(string id)
        {
            var employee = _employeeContext.Employees
            .Include(e => e.DirectReports) // Batch load DirectReports
            .ThenInclude(dr => dr.DirectReports) // Batch load DirectReports' DirectReports
            .SingleOrDefault(e => e.EmployeeId == id);

            return employee;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
