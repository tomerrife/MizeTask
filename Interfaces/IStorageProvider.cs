using System;
using System.Threading.Tasks;

namespace Chain.Interfaces
{
    public interface IStorageProvider<T> where T : class
    {
        Task<T> Read();
    }

    public interface IWritableStorageProvider<T> : IStorageProvider<T> where T : class
    {
        TimeSpan ExpirationTime { get; set; }
        Task Write(T value);
    }
}
