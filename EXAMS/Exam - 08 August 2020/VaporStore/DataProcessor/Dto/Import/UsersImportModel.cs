using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class UsersImportModel
    {
        [Required]
        [RegularExpression(@"^[A-Z][a-z]{2,}\s[A-Z][a-z]{2,}$")]
        public string FullName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3, 103)]
        public int Age { get; set; }

        public CardsImportModel[] Cards { get; set; }
    }

    public class CardsImportModel
    {
        [Required]
        [RegularExpression(@"\d{4} \d{4} \d{4} \d{4}")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"\d{3}")]
        public string CVC { get; set; }

        [Required]
        public CardType? Type { get; set; }
    }
}
