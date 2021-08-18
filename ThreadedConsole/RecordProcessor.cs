using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThreadedConsole.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

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
        public async void ThreadPoolCallback(Object threadContext)
        {
            //Request that the processor is executed and update the doneEvent once complete
            await this.Execute();
            _doneEvent.Set();
        }

        /// <summary>
        /// Method to handle the process execution events!
        /// i.e. provide a delay and read through the list of email records
        /// </summary>
        private async Task Execute()
        {
            Console.WriteLine("Execute - Thread {0} started", _threadNumber);
            // var rand = new Random();
            // Thread.Sleep(rand.Next(1, 10) * 100);

            foreach (var record in _records)
            {
                //Console.WriteLine("Process - Thread {0} - {1}", _threadNumber, record.Email);
                if (record.Courier.Trim().ToUpper() == "DPD")
                {
                    if (await SendDPDOrder(record) == false)
                    {
                        Console.WriteLine($"Failed DPD Email {record.Email}");
                    }
                    // else
                    // {
                    //     Console.WriteLine($"Sent DPD Email {record.Email}");
                    // }
                }
                else
                {
                    Console.WriteLine($"Unable to send to {record.Email}");
                }
            }
        }

        async Task<bool> SendDPDOrder(CSVRecord record)
        {
            //Console.WriteLine($"SendDPDOrder : {record.Email}");
            try
            {
                //Send email
                var apiKey = "";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("no-reply@abc.com", "ABC");
                var to = new EmailAddress(record.Email.Trim());
                var subject = $"Sample Email";
                var plainTextContent = $@"Message:";
                var htmlContent = "";

                htmlContent = $@"<p>Dear Customer,</p>
                            <p>Thank you for your order with User!</p>

                            <p>Please do not reply to this email</p>

                            <p>Kind regards,</p>
                            <p><strong>ABC</strong></p>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return false;
                }

                return (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                Console.WriteLine($"SendDPDOrder error sending an email to {record.Email} with exception: {e.Message}");
                return false;
            }
        }
    }
}