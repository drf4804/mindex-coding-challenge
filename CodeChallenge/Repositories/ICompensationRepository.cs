using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        Compensation GetById(string id);
        Task SaveAsync();
        Compensation Remove(Compensation compensation);
    }
}
