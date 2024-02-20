using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly CompensationContext _compensationContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, CompensationContext compensationContext)
        {
            _compensationContext = compensationContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            _compensationContext.Compensations.Add(compensation);
            return compensation;
        }

        /// <summary>
        /// A method that will obtain the Compensation via the EmployeeID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Compensation GetById(string id)
        {
            // I tried a different method of loading the data because I'm not a huge fan of using Include(). I also haven't worked with
            // DbContexts before, so I'd love to eventually hear what are some best practices here as well!
            var compensation = _compensationContext.Compensations
                .SingleOrDefault(c => c.EmployeeId == id);

            if (compensation != null)
            {
                _compensationContext.Entry(compensation)
                    .Reference(c => c.Employee)
                    .Load();

                if (compensation.Employee != null)
                {
                    _compensationContext.Entry(compensation.Employee)
                        .Collection(e => e.DirectReports)
                        .Load();

                    if (compensation.Employee.DirectReports != null)
                    {
                        foreach (var directReport in compensation.Employee.DirectReports)
                        {
                            _compensationContext.Entry(directReport)
                                .Collection(dr => dr.DirectReports)                                
                                .Load();
                        }
                    }
                }
            }

            return compensation;
        }

        public Task SaveAsync()
        {
            return _compensationContext.SaveChangesAsync();
        }

        public Compensation Remove(Compensation compensation)
        {
            return _compensationContext.Remove(compensation).Entity;
        }
    }
}
