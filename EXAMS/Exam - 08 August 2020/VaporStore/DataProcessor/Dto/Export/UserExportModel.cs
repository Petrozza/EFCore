using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class UserExportModel
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public PurchaseExportModel[] Purchases { get; set; }

        [XmlElement("TotalSpent")]
        public decimal TotalSpent { get; set; }
    }

    [XmlType("Purchase")]
    public class PurchaseExportModel
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Game")]
        public GameExportModel Game { get; set; }
    }

    [XmlType("Game")]
    public class GameExportModel
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
//</User>
//  <User username="vsjollema">
//    <Purchases>
//      <Purchase>
//        <Card>8608 6806 8238 3092</Card>
//        <Cvc>081</Cvc>
//        <Date>2017-10-01 01:14</Date>
//        <Game title="Garry's Mod">
//          <Genre>Indie</Genre>
//          <Price>9.99</Price>
//        </Game>
//      </Purchase>
//      <Purchase>
//        <Card>4846 1275 4235 3039</Card>
//        <Cvc>268</Cvc>
//        <Date>2017-11-12 03:51</Date>
//        <Game title="Total War: WARHAMMER II">
//          <Genre>Action</Genre>
//          <Price>59.99</Price>
//        </Game>
//      </Purchase>
//    </Purchases>
//    <TotalSpent>69.98</TotalSpent>
//  </User>

