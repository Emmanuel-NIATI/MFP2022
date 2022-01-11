using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Microbit
{
    public partial class DeviceHelpers
    {

        public static async Task<T> GetFirstDeviceAsync<T>(string selector, Func<string, Task<T>> convertAsync) where T : class
        {

            var completionSource = new TaskCompletionSource<T>();
            var pendingTasks = new List<Task>();
            DeviceWatcher watcher = DeviceInformation.CreateWatcher(selector);

            watcher.Added += (DeviceWatcher sender, DeviceInformation device) =>
            {

                Func<string, Task> lambda = async (id) =>
                {

                    T t = await convertAsync(id);
                    if (t != null)
                    {
                        completionSource.TrySetResult(t);
                    }

                };

                pendingTasks.Add(lambda(device.Id));

            };

            watcher.EnumerationCompleted += async (DeviceWatcher sender, object args) =>
            {
                
                await Task.WhenAll(pendingTasks);
                
                completionSource.TrySetResult(null);

            };

            watcher.Start();

            T result = await completionSource.Task;

            watcher.Stop();

            return result;

        }

    }

}
