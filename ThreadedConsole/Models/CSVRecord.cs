using System;

namespace ThreadedConsole.Models
{
    /// <summary>
    /// Demo CSV record model file
    /// Now converted to a record!
    /// </summary>
    public record CSVRecord
    {
        public string OrderNumber { get; set; }
        public int TrackingNumber { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Booking reference for the customer
        /// </summary>
        public string FormattedTrackingNumber => TrackingNumber.ToString("D8");

        /// <summary>
        /// Statically create a CSVrecord from the incoming csvLine data
        /// </summary>
        /// <param name="csvLine">single CSV line record</param>
        /// <returns>completed CSVRecord</returns>
        public static CSVRecord FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            CSVRecord rec = new CSVRecord();
            rec.OrderNumber = values[0];
            rec.TrackingNumber = Convert.ToInt32(values[1]);
            //rec.Courier = values[2];
            rec.Email = values[2];
            return rec;
        }
    }
}