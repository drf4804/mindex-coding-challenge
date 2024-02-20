using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ICompensationRepository compensationRepository, ILogger<CompensationService> logger) 
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }
        /// <summary>
        /// A method that will create a Compensation based on the given id and Compensation object passed in. Compensation will then be added to
        /// the CompensationContext to persist.
        /// </summary>
        /// <param name="id">The employeeId that will have a compensation created</param>
        /// <param name="compensation">The compensation that will be created for the given employee via employeeId</param>
        /// <returns></returns>
        public Compensation Create(string id, Compensation compensation)
        {
            if (compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }
            return compensation; 
        }

        public Compensation GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _compensationRepository.GetById(id);
            }

            return null;
        }
    }
}
