using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Chain.Interfaces;

namespace Chain.Infrastructure
{
    public class ChainResource<T> : IChainResource<T> where T : class
    {
        private static ChainResource<T> SingletonInstance;
        private readonly IEnumerable<IStorageProvider<T>> StorageList;
        private static readonly object LockObj = new object();

        private ChainResource(IEnumerable<IStorageProvider<T>> storageList)
        {
            this.StorageList = storageList;
        }

        public static ChainResource<T> GetInstace(IEnumerable<IStorageProvider<T>> storageList)
        {
                if (SingletonInstance == null)
                {
                    lock (LockObj)
                    {
                        if (SingletonInstance == null)
                        {
                            SingletonInstance = new ChainResource<T>(storageList);
                        }
                    }
                }
                return SingletonInstance;
        }

        public async Task<T> GetValue()
        {
            foreach (var storage in StorageList)
            {
                var readResult = await storage.Read();
                if (readResult != null)
                {
                    foreach (var writeStorage in StorageList.OfType<IWritableStorageProvider<T>>())
                    {
                        await writeStorage.Write(readResult);
                    }

                    return readResult;
                }
            }

            return null;
        }
    }
}
