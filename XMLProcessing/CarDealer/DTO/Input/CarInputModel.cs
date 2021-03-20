using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Input
{
    [XmlType("Car")]
    public class CarInputModel
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("TraveledDistance")]
        public long TravelDistance { get; set; }
        [XmlArray("parts")]
        public CarPartsInputModel[] CarPartsInputModel { get; set; }
    }
    [XmlType("partId")]
    public class CarPartsInputModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
