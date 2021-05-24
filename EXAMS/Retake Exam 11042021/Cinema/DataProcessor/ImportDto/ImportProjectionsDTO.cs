using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Projection")]
    public class ImportProjectionsDTO
    {
        [XmlElement("MovieId")]
        //[ForeignKey("Movie")]
        public int MovieId { get; set; }


        [XmlElement("DateTime")]
        [Required]
        public string DateTime { get; set; }
    }
}
