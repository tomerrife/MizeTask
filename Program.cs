using Chain.Domain;
using Chain.Infrastructure;
using Chain.Interfaces;
using Chain.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MizeTask
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var memoryStorage = new MemoryStorage<ExchangeRateList>("key", new TimeSpan(1,0,0));
            var fileSystemStorage = new FileSystemSrorage<ExchangeRateList>("exchangeRates.json", new TimeSpan(4, 0, 0));
            var webServiceStorage = new OpenExchangeRateService<ExchangeRateList>("fb2eb11772b84facbcfd4e252029e641");
            var storages = new List<IStorageProvider<ExchangeRateList>> { memoryStorage, fileSystemStorage, webServiceStorage };

            var chainResource = ChainResource<ExchangeRateList>.GetInstace(storages);
            await chainResource.GetValue();
        }
    }
}
