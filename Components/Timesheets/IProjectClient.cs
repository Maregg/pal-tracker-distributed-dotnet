using System.Threading.Tasks;

namespace Timesheets
{
    public interface IProjectClient
    {
        Task<ProjectInfo> GetAsync(long projectId);
    }
}