using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeFirst.Models
{
    public class News
    {

        public News()
        {
            Comments = new HashSet<Comments>();
        }
        public int Id { get; set; }

        [MaxLength(300)]
        public string Title { get; set; }

        public string Content { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<Comments> Comments { get; set; }
    }
}
