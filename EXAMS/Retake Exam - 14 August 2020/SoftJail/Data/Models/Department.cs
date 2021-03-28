using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(3, 25)]
        public string Name { get; set; }

        public virtual ICollection<Cell> Cells { get; set; } = new HashSet<Cell>();
    }
}
