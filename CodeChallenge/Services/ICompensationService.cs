using CodeChallenge.Models;
using System;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        public Compensation Create(String id, Compensation compensation);
        Compensation GetById(string id);
    }
}
