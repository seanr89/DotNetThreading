using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utilities;
using ThreadedConsole.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThreadedConsole
{
    class Program
    {
        //private static IConfigurationRoot Configuration { get; set; }
        async static Task Main(string[] args)
        {
            Console.WriteLine("Thread Runner");
            Console.WriteLine("==========================");

            //Step1. open and process the file
            string path = "/Users/Public/Downloads/MOCK_DATA.csv";
            //string path = "/Users/Public/Downloads/DPD17Aug2021-Short100.csv";
            List<CSVRecord> values = System.IO.File.ReadAllLines(path)
                                        .Skip(1)
                                        .Select(v => CSVRecord.FromCsv(v))
                                        .ToList();

            Console.WriteLine("Loaded {0} records", values.Count);

            var splitRecords = HelperMethods.SplitList(values, 200).ToList();

            try
            {
                using (ExecutionPerformanceMonitor monitor = new ExecutionPerformanceMonitor())
                {
                    await RunThreadPoolTest(splitRecords);
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

            Console.WriteLine("App Completed - Press Any Key to Finish");
            Console.ReadKey();
        }

        /// <summary>
        /// Supports thread pool execution for the configured data
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        static async Task<string> RunThreadPoolTest(List<List<CSVRecord>> values)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool.queueuserworkitem?view=net-5.0
            //Wrapped in an await now but perhaps not required!
            return await Task.Run(() =>
            {
                Console.WriteLine($"RunThreadPoolTest");
                //initialise and the min and max thread pool count!
                ThreadPool.SetMinThreads(1, 1);
                ThreadPool.SetMaxThreads(4, 4);
                //initialise a done event handler array set for controlling/delaying completion
                var doneEvents = new ManualResetEvent[values.Count];
                for (int i = 0; i < values.Count; i++)
                {
                    //set the done event, initialise the processor and work!
                    doneEvents[i] = new ManualResetEvent(false);
                    var f = new RecordProcessor(values[i], i, doneEvents[i]);
                    // ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(f.ThreadPoolCallback), i);
                }

                //WaitHandler to block until all the work is done
                WaitHandle.WaitAll(doneEvents);
                return "Job Done";
            });
        }
    }
}
