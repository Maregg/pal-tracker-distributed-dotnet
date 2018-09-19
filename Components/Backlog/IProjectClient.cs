using System.Threading.Tasks;

namespace Backlog
{
    public interface IProjectClient
    {
        Task<ProjectInfo> GetAsync(long projectId);
    }
}