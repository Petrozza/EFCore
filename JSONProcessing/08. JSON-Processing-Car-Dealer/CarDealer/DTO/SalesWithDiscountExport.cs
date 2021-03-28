using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class SalesWithDiscountExport
    {
        [JsonProperty("car")]
        public SaleCar  Car { get; set; }
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }
        [JsonProperty("Discount")]
        public string Discount { get; set; }
        [JsonProperty("price")]
        public string Price { get; set; }
        [JsonProperty("priceWithDiscount")]
        public string PriceWithDiscount { get; set; }
    }

    public class SaleCar
    {
        [JsonProperty("Make")]
        public string Make { get; set; }
        [JsonProperty("Model")]
        public string Model { get; set; }
        [JsonProperty("TravelledDistance")]
        public long TravelledDistance { get; set; }
    }
}
