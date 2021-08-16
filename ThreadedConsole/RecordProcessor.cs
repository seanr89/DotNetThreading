using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThreadedConsole.Models;

namespace ThreadedConsole
{
    public class RecordProcessor
    {
        private ManualResetEvent _doneEvent;
        private readonly List<CSVRecord> _records;
        private readonly int _threadNumber;
        public RecordProcessor(List<CSVRecord> records, int threadNumber, ManualResetEvent doneEvent)
        {
            _doneEvent = doneEvent;
            _records = records;
            _threadNumber = threadNumber;
        }

        /// <summary>
        /// ThreadPool queue event to be executed
        /// </summary>
        /// <param name="threadContext">Incoming threadContext and its parameters</param>
        public void ThreadPoolCallback(Object threadContext)
        {
            //Request that the processor is executed and update the doneEvent once complete
            this.Execute();
            _doneEvent.Set();
        }

        /// <summary>
        /// Method to handle the process execution events!
        /// i.e. provide a delay and read through the list of email records
        /// </summary>
        private void Execute()
        {
            Console.WriteLine("Execute - Thread {0} started", _threadNumber);
            var rand = new Random();
            Thread.Sleep(rand.Next(1, 10) * 100);

            foreach (var record in _records)
            {
                Console.WriteLine("Process - Thread {0} - {1}", _threadNumber, record.Email);
            }
        }
    }
}