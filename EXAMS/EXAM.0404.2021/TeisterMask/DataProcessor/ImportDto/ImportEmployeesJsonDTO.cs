using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    
    class ImportEmployeesJsonDTO
    {
        
        [MinLength(3)]
        [MaxLength(40)]
        [Required]
        [RegularExpression(@"[a-zA-Z]+[0-9]+")]
        public string Username { get; set; }

        
        [EmailAddress]
        [Required]
        public string Email { get; set; }


        
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$")]
        [Required]
        public string Phone { get; set; }

        
        public int[] Tasks { get; set; }
    }
}
