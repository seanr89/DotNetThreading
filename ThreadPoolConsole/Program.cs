using System.Diagnostics;

namespace ThreadPoolConsole
{
    class Program
    {
        //private static IConfigurationRoot Configuration { get; set; }
        async static Task Main(string[] args)
        {
            Console.WriteLine("Thread Pool Started");

            var sw = new Stopwatch();
            sw.Start();

            int workers, ports;
    
            // Get minimum number of threads  
            ThreadPool.GetMinThreads(out workers, out ports);  
            Console.WriteLine($"Minimum worker threads: {workers} ");  
            Console.WriteLine($"Minimum completion port threads: {ports}");  
  
            //Console.ReadKey();
            
            for(int i = 0; i < 15; i++)
            {
                Console.WriteLine($"Iteration: {i}");
                ThreadPool.QueueUserWorkItem(Job);
                Thread.Sleep(200);
            }

            static void Job(object state){
                for (int i = 0; i < 4; i++)
                {
                    Console.WriteLine("cycle {0}, is processing by thread {1}",
                    i, Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(350);
                }
            }

            sw.Stop();
            Console.WriteLine($"Steps complete taking {sw.ElapsedMilliseconds}ms");
            
            Console.ReadKey(); 


            Console.WriteLine("App Complete");
        }
    }
}