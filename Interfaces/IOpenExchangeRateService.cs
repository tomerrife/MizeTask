using System.Threading.Tasks;

namespace Chain.Interfaces
{
    public interface IOpenExchangeRateService<T>
    {
        Task<T> GetLatestExchangeRates();
    }
}
