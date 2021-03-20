using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P03_FootballBetting
{
    public class Country
    {
        public int CountryId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Town> Towns { get; set; }
    }
}
