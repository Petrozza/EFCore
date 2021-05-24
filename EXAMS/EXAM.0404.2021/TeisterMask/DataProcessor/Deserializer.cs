namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var projects = new List<Project>();

            var xmlSerializer = new XmlSerializer(typeof(ImportProjestsDTO[]), new XmlRootAttribute("Projects"));
            var textReader = new StringReader(xmlString);
            var importedProjects = (ImportProjestsDTO[])xmlSerializer.Deserialize(textReader);

            foreach (var currProject in importedProjects)
            {

                if (!IsValid(currProject))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isDueDateValid = DateTime.TryParseExact(currProject.DueDate, 
                    "dd/MM/yyyy", 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out var validDueDate);

                Project project = new Project()
                {
                    Name = currProject.Name,
                    OpenDate = DateTime.ParseExact(currProject.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DueDate = isDueDateValid ? (DateTime?)validDueDate : null
                };

                foreach (var currTask in currProject.Tasks)
                {
                    if (!IsValid(currTask))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var validTaskOpenDate = DateTime.ParseExact(currTask.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var validTaskDueDate = DateTime.ParseExact(currTask.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);



                    if (validTaskOpenDate <= project.OpenDate || validTaskDueDate >= project.DueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new Task()
                    {
                        Name = currTask.Name,
                        OpenDate = validTaskOpenDate, 
                        DueDate = validTaskDueDate, 
                        ExecutionType = Enum.Parse<ExecutionType>(currTask.ExecutionType),
                        LabelType = Enum.Parse<LabelType>(currTask.LabelType),
                        Project = project
                    };

                    project.Tasks.Add(task);

                    
                }
                projects.Add(project);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));

            }
            context.Projects.AddRange(projects);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var empToImport = JsonConvert.DeserializeObject<ImportEmployeDTO[]>(jsonString);

            foreach (var emp in empToImport)
            {
                if (!IsValid(emp))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var employee = new Employee
                {
                    Username = emp.Username,
                    Email = emp.Email,
                    Phone = emp.Phone,
                };

                var tasks = context.Tasks.Select(t => t.Id).ToList();

                foreach (var task in emp.Tasks.Distinct())
                {
                    if ( !tasks.Contains(task))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask
                    {
                        TaskId = task
                    });
                }

                context.Employees.Add(employee);
                context.SaveChanges();
                sb.AppendLine($"Successfully imported employee - {employee.Username} with {employee.EmployeesTasks.Count} tasks.");
            }
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}