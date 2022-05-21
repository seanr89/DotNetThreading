
namespace ThreadPoolConsole
{
    class Program
    {
        //private static IConfigurationRoot Configuration { get; set; }
        async static Task Main(string[] args)
        {
            Console.WriteLine("Thread Pool Started");

            int workers, ports;
            ThreadPool.SetMaxThreads(1,2);
            ThreadPool.SetMinThreads(1,1);
  
            // Get maximum number of threads  
            ThreadPool.GetMaxThreads(out workers, out ports);  
            // Console.WriteLine($"Maximum worker threads: {workers} ");  
            // Console.WriteLine($"Maximum completion port threads: {ports}");  
        
            // Get available threads  
            ThreadPool.GetAvailableThreads(out workers, out ports);  
            // Console.WriteLine($"Available worker threads: {workers} ");  
            // Console.WriteLine($"Available completion port threads: {ports}");  
    
            // Set minimum threads  
            int minWorker, minIOC;  
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMinThreads(4, minIOC);  
  
            // Get total number of processes availalbe on the machine  
            // int processCount = Environment.ProcessorCount;  
            // Console.WriteLine($"No. of processes available on the system: {processCount}");  
    
            // Get minimum number of threads  
            ThreadPool.GetMinThreads(out workers, out ports);  
            Console.WriteLine($"Minimum worker threads: {workers} ");  
            Console.WriteLine($"Minimum completion port threads: {ports}");  
  
            //Console.ReadKey();
            
            for(int i = 0; i < 15; i++)
            {
                Console.WriteLine($"Iteration: {i}");
                ThreadPool.QueueUserWorkItem(Job);
                Thread.Sleep(500);
            }

            static void Job(object state){
                for (int i = 0; i < 4; i++)
                {
                    Console.WriteLine("cycle {0}, is processing by thread {1}",
                    i, Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(350);
                }
            }

            Console.WriteLine("Steps complete");
            Console.ReadKey(); 


            Console.WriteLine("App Complete");
        }
    }
}