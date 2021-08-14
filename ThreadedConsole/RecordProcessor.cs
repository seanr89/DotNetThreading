using System;
using System.Collections.Generic;
using System.Threading;
using ThreadedConsole.Models;

namespace ThreadedConsole
{
    public class RecordProcessor
    {
        public RecordProcessor()
        {

        }

        public void Execute(List<CSVRecord> records, int threadNumber)
        {
            Console.WriteLine("Process - Thread {0} started", threadNumber);
            var rand = new Random();
            Thread.Sleep(rand.Next(10, 50) * 10);

            foreach (var record in records)
            {
                Console.WriteLine("Process - Thread {0} - {1}", threadNumber, record.Email);
            }
        }
    }
}