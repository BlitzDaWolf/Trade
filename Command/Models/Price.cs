using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.Models
{
    public class Price
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Symbol { get; set; }
        public DateTime Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double TickVolume { get; set; }

        public PriceData ToRPC()
        {
            return new PriceData
            {
                Close = Close,
                High = High,
                Low = Low,
                Open = Open,
                Symbol = Symbol,
                Time = Time.Ticks
            };
        }

        public Quote ToQuote()
        {
            return new Quote
            {

                Close = (decimal)Close,
                High = (decimal)High,
                Low = (decimal)Low,
                Open = (decimal)Open,
                Volume = Time.Ticks
            };
        }

        public static Price FromRpc(CollectionPrice collectionPrice)
        {
            return new Price
            {
                Close = collectionPrice.Close,
                High = collectionPrice.High,
                Low = collectionPrice.Low,
                Open = collectionPrice.Open,
                Symbol = collectionPrice.Symbol,
                TickVolume = 0,
                Time = new DateTime(collectionPrice.Time)
            };
        }
    }
}
