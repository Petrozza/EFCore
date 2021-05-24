﻿using Cinema.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public double Rating { get; set; }

        [Required]
        public string Director { get; set; }
        public virtual ICollection<Projection> Projections { get; set; } = new HashSet<Projection>();
    }
}
