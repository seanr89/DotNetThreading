
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelConsole;

class Program
{
    async static Task Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, World!");

        await RunAsyncParallelJob();
    }

    static async Task RunAsyncParallelJob()
    {
        var listData = createList(200);

        await Parallel.ForEachAsync(listData, async (item, cancellationToken) =>
        {   
            Console.WriteLine($"Called for {item.name}, will wait {item.waitTime} ms");
            await Task.Delay(item.waitTime);
            Console.WriteLine($"Done handling {item.name}");
        });
    }

    static List<(string name, int waitTime)> createList(int max = 1000)
    {
        List<(string name, int waitTime)> data = new List<(string name, int waitTime)>();
        
        int count = 0;
        var rdm = new Random();
        do{
            int value = rdm.Next(500, 5000);
            data.Add(new ($"item {count}", value));
            count++;
        }while(count < max);

        return data;
    }
}