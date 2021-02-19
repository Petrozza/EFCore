using System;
using System.Collections.Generic;

#nullable disable

namespace Simple.Exams
{
    public partial class VRank
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Salary { get; set; }
        public long? Rank { get; set; }
    }
}
