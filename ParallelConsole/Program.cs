
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;

namespace ParallelConsole;

class Program
{
    async static Task Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information
        Console.Clear();
        Console.WriteLine("Starting Parallel App");

        var listData = createList(500);

        using (ExecutionPerformanceMonitor monitor = new ExecutionPerformanceMonitor())
        {
            await RunAsyncParallelJob(listData);
            Console.WriteLine($"{monitor.CreatePerformanceTimeMessage("Async Parallel Runner")}");
        }

        using (ExecutionPerformanceMonitor monitor2 = new ExecutionPerformanceMonitor())
        {
            await RunNormalListProcess(listData);
            Console.WriteLine($"{monitor2.CreatePerformanceTimeMessage("Standard Runner")}");
        }

        Console.WriteLine("App Completed");
    }

    /// <summary>
    /// Async job to process an auto generated list in a parallel process
    /// </summary>
    /// <returns></returns>
    static async Task RunAsyncParallelJob(List<(string name, int waitTime)> listData)
    {
        int count = 0;
        await Parallel.ForEachAsync(listData, async (item, cancellationToken) =>
        {   
            //Console.WriteLine($"Called for {item.name}, will wait {item.waitTime} ms");
            await Task.Delay(item.waitTime);
            count += item.waitTime;
            //Console.WriteLine($"Aysnc Done handling {item.name}");
        });
        Console.WriteLine($"Count: {count}");
    }

    static async Task RunNormalListProcess(List<(string name, int waitTime)> listdata)
    {
        int count = 0;
        // await Task.Run(() => {
        foreach(var x in listdata){
            await Task.Run(async () => {
                await Task.Delay(x.waitTime);
                count += x.waitTime;
            });
            //Console.WriteLine($"Done handling {x.name}");
        };
            //});
        //});
        Console.WriteLine($"Count: {count}");
    }

    /// <summary>
    /// Creates a dummy list of items with a auto generated delay
    /// </summary>
    /// <param name="name">dummy list of operations</param>
    /// <param name="max"></param>
    /// <returns></returns>
    static List<(string name, int waitTime)> createList(int max = 1000)
    {
        List<(string name, int waitTime)> data = new List<(string name, int waitTime)>();
        
        int count = 0;
        var rdm = new Random();
        do{
            int value = rdm.Next(20, 100);
            data.Add(new ($"item {count}", value));
            count++;
        }while(count < max);

        return data;
    }
}