using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(GetLatestProjects(context));
        }


        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(z => new
                {
                    z.Name,
                    z.Description,
                    SStartDate = z.StartDate.ToString("M/d/yyyy h:mm:ss tt"
                        , CultureInfo.InvariantCulture)
                })
                .OrderBy(r => r.Name)
                .ToList();

            var sb = new StringBuilder();
            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.SStartDate}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(z => z.Name)
                .Select(z => new
                {
                    z.Name,
                    ManagerFirstName = z.Manager.FirstName,
                    ManagerLastName = z.Manager.LastName,
                    Employees = z.Employees.Select(y => new
                    {
                        y.FirstName,
                        y.LastName,
                        y.JobTitle
                    })
                    .OrderBy(x => x.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList()
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.Name} – {dep.ManagerFirstName} {dep.ManagerLastName}");

                foreach (var emp in dep.Employees)
                {
                    sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();

        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects
                        .Select(y => y.Project.Name)
                        .OrderBy(z => z)
                        .ToList()
                })
                .Single();
                //.FirstOrDefault(e => e.EmployeeId == 147);

            var sb = new StringBuilder();
            sb.AppendLine($"{employee147.FirstName} {employee147.LastName} - {employee147.JobTitle}");

            foreach (var pr in employee147.Projects)
            {
                sb.AppendLine(pr);
            }

            return sb.ToString().TrimEnd();


        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var adressess = context.Addresses
                .Select(e => new
                {
                    Count = e.Employees.Count,
                    TownName = e.Town.Name,
                    AdressText = e.AddressText
                })
                .OrderByDescending(a => a.Count)
                .ThenBy(t => t.TownName)
                .ThenBy(t => t.AdressText)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var adr in adressess)
            {
                sb.AppendLine($"{adr.AdressText}, {adr.TownName} - {adr.Count} employees");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesProjects
                .Any(ep => ep.Project.StartDate.Year >= 2001
                && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(m => new
                {
                    m.FirstName,
                    m.LastName,
                    ManagerFirstName = m.Manager.FirstName,
                    ManagerLastNane = m.Manager.LastName,
                    Projects = m.EmployeesProjects
                    .Select(y => new
                    {
                        ProjectName = y.Project.Name,
                        StartDate = y.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"
                        , CultureInfo.InvariantCulture),
                        EndDate = y.Project.EndDate.HasValue ?
                        y.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt"
                        , CultureInfo.InvariantCulture) : "not finished"
                    })
                    .ToList()
            }) 
            .ToList();

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastNane}");

                foreach (var p in employee.Projects)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();

        }

       

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(address);
            context.SaveChanges();

            var employee = context.Employees
                          .FirstOrDefault(x => x.LastName == "Nakov");
            
            employee.AddressId = address.AddressId;
            context.SaveChanges();

            var addresses = context.Employees
                    .Select(x => new 
                {
                    x.Address.AddressText,
                    x.Address.AddressId
                })
                    .OrderByDescending(x => x.AddressId)
                    .Take(10)
                    .ToList();

            var sb = new StringBuilder();
            foreach (var pich in addresses)
            {
                sb.AppendLine(pich.AddressText);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                            .Where(x => x.Department.Name == "Research and Development")
                            .OrderBy(x => x.Salary)
                            .ThenByDescending(x => x.FirstName)
                            .Select(x => new
                            {
                                x.FirstName,
                                x.LastName,
                                x.Department.Name,
                                x.Salary,

                            });

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Name} - ${employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                            .Where(x => x.Salary > 50000)
                            .Select(x => new
                            {
                                x.FirstName,
                                x.Salary
                            })
                            .OrderBy(x => x.FirstName);

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        
        
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(x => x.EmployeeId)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.MiddleName,
                    x.JobTitle,
                    x.Salary
                });

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
