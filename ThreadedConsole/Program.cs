﻿using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utilities;
using ThreadedConsole.Models;
using System.Collections.Generic;
using System.Linq;

namespace ThreadedConsole
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Thread Runner");
            Configuration = LoadAppSettings();
            var serviceCollection = new ServiceCollection();
            RegisterAndInjectServices(serviceCollection, Configuration);
            //Initialise netcore dependency injection provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //Step1. open and process the file
            string path = "/Users/Public/Downloads/mock_data.csv";
            List<CSVRecord> values = System.IO.File.ReadAllLines(path)
                                        .Skip(1)
                                        .Select(v => CSVRecord.FromCsv(v))
                                        .ToList();

            Console.WriteLine("Loaded {0} records", values.Count);

            var splitRecords = HelperMethods.SplitList(values, 500).ToList();

            try
            {
                using (ExecutionPerformanceMonitor monitor = new ExecutionPerformanceMonitor())
                {
                    //RunThreadTest();
                    RunThreadPoolTest(splitRecords);
                    Console.ReadKey();
                    Console.WriteLine($"{monitor.CreatePerformanceTimeMessage("Thread Runner")}");
                }
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine($"Implementation Exception caught: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Generic Exception caught: {e.Message}");
            }

            Console.WriteLine("App Completed");
        }

        // void RunThreadTest()
        // {
        //     Thread th = Thread.CurrentThread;
        //     th.Name = "MainThread";
        //     Console.WriteLine("This is {0}", th.Name);
        //     Console.ReadKey();
        //     //
        // }

        static void RunThreadPoolTest(List<List<CSVRecord>> values)
        {
            Console.WriteLine($"RunThreadPoolTest with recordList {values.Count}");
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(4, 4);
            //int workers, ports;
            // Get maximum number of threads  
            //ThreadPool.GetMaxThreads(out workers, out ports);
            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool.queueuserworkitem?view=net-5.0
            //Console.WriteLine($"ThreadPool Test. Count {workers}");

            for (int i = 0; i < values.Count; i++)
            {
                //Console.WriteLine($"size {values[i]} and position {i}");
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), new object[] { values[i], i });
                //Console.WriteLine("Main thread does some work, then sleeps.");
            }
        }

        // This thread procedure performs the task.
        static void ThreadProc(object package)
        {
            //Console.WriteLine($"ThreadProc");
            var castedPackage = (object[])package;
            int threadIndex = (int)castedPackage[1];
            Console.WriteLine($"Starting Thread: {threadIndex}");
            // No state object was passed to QueueUserWorkItem, so stateInfo is null.
            var recorder = new RecordProcessor();
            recorder.Execute((List<CSVRecord>)castedPackage[0], threadIndex);

            //Console.WriteLine($"Completing Thread {threadIndex}");
        }

        /// <summary>
        /// Query app settings json content
        /// </summary>
        /// <returns></returns>
        private static IConfigurationRoot LoadAppSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Error trying to load app settings");
                return null;
            }
        }

        /// <summary>
        /// Prep/Configure Dependency Injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        private static void RegisterAndInjectServices(IServiceCollection services, IConfiguration config)
        {
            // services.AddLogging(logging =>
            // {
            //     logging.AddConsole();
            // }).Configure<LoggerFilterOptions>(options => options.MinLevel =
            //                                     LogLevel.Warning);

            services.AddTransient<RecordProcessor>();
        }

    }
}
