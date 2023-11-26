using System.Threading.Tasks;

namespace Chain.Interfaces
{
    public interface IChainResource<T>
    {
        Task<T> GetValue();
    }
}
