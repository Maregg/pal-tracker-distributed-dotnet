using System.Threading.Tasks;

namespace Allocations
{
    public interface IProjectClient
    {
        Task<ProjectInfo> GetAsync(long projectId);
    }
}