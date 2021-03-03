using System;
using System.Linq;

namespace Simple.Exams
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SoftUniContext();
            var department = db.Employees.GroupBy(x => x.Department.Name)
                .Select(x => new { Name = x.Key, Count = x.Count() }).ToList();
            foreach (var Otdel in department)
            {
                Console.WriteLine($"{Otdel.Name} - {Otdel.Count}");
            }
        }
    }
}
