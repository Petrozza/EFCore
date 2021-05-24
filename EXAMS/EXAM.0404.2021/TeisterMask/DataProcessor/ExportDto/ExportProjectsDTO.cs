using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project ")]
    public class ExportProjectsDTO
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        [XmlElement("ProjectName")]
        [MinLength(2)]
        [MaxLength(40)]
        [Required]
        public string ProjectName { get; set; }

        [XmlElement("HasEndDate")]
        [MinLength(2)]
        [MaxLength(3)]
        [Required]
        public string HasEndDate { get; set; }

        [XmlArray("Tasks")]
        public ImportXmlTaskDTO[] Tasks { get; set; }
    }
    [XmlType("Task")]
    public class ImportXmlTaskDTO
    {
        [XmlElement("Name")]
        [MinLength(2)]
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }
    }
}
