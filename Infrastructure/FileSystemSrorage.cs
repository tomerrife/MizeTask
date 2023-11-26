using Chain.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chain.Infrastructure
{
    public class FileSystemSrorage<T> : IWritableStorageProvider<T> where T : class
    {
        private readonly string FilePath;

        private readonly object LockjObj = new object();

        private TimeSpan expirationTime;
        public FileSystemSrorage(string filePath, TimeSpan expirationTime)
        {
            FilePath = filePath;
            this.expirationTime = expirationTime;
        }

        public TimeSpan ExpirationTime { get => expirationTime; set => expirationTime = value; }

        public Task<T> Read()
        {
            lock (LockjObj)
            {
                if (!File.Exists(FilePath))
                {
                    return Task.FromResult<T>(null);
                }

                var lastWriteTime = File.GetLastWriteTime(FilePath);
                var expirationDate = lastWriteTime.Add(expirationTime);

                if (DateTime.Now > expirationDate)
                {
                    Console.WriteLine("FileSystemSrorage file expiration time expired");
                    return Task.FromResult<T>(null);
                }

                var json = File.ReadAllText(FilePath);
                var result = JsonSerializer.Deserialize<T>(json);

                return Task.FromResult(result);
            }
        }


        public Task Write(T value)
        {
            bool isExpired = false;
            if (File.Exists(FilePath))
            {
                var lastWriteTime = File.GetLastWriteTime(FilePath);
                var expirationDate = lastWriteTime.Add(expirationTime);
                isExpired = DateTime.Now > expirationDate;
            }
            lock (LockjObj)
            {
                if (!File.Exists(FilePath) || isExpired)
                {
                    var json = JsonSerializer.Serialize(value);
                    File.WriteAllText(FilePath, json);
                    Console.WriteLine("FileSystem stored new file");
                }
            }
            return Task.CompletedTask;
        }
    }
}
