namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            List<Department> departments = new List<Department>();

            DepartmentsCellsImportDTO[] departmentDtos = JsonConvert.DeserializeObject<DepartmentsCellsImportDTO[]>(jsonString);

            foreach (var departmentDto in departmentDtos)
            {
                if (!IsValid(departmentDto) || !departmentDto.Cells.Any() /*(x => IsValid(x))*/
                    || !departmentDto.Cells.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Department department = new Department()
                {
                    Name = departmentDto.Name,
                    Cells = departmentDto.Cells.Select(x => new Cell 
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    })
                    .ToArray()
                };

                departments.Add(department);
                //List<Cell> cells = new List<Cell>();
                //foreach (var cellDto in departmentDto.Cells)
                //{
                //    if (!IsValid(cellDto))
                //    {
                //        cells = new List<Cell>();
                //        break;
                //    }

                //    Cell cell = new Cell()
                //    {
                //        CellNumber = cellDto.CellNumber,
                //        Department = department,
                //        HasWindow = cellDto.HasWindow
                //    };
                //    cells.Add(cell);

                //}

                //if (!cells.Any())
                //{
                //    sb.AppendLine("Invalid Data");
                //    continue;
                //}

                //department.Cells = cells;
                //departments.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");

            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().Trim();

        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var prisoners = new List<Prisoner>();
            var prisonersDtos = JsonConvert.DeserializeObject<ImportPrisonersDTO[]>(jsonString);

            foreach (var prisonerDto in prisonersDtos)
            {
                if (!IsValid(prisonerDto)
                   || !prisonerDto.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var isvalidReleaseDate = DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDate);

                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId,
                    IncarcerationDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),

                    ReleaseDate = isvalidReleaseDate ? (DateTime?)releaseDate : null,
                    Mails = prisonerDto.Mails.Select(m => new Mail
                    {
                        Description = m.Description,
                        Sender = m.Sender,
                        Address = m.Address
                    })
                    .ToArray()
                };
                prisoners.Add(prisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validOfficers = new List<Officer>();
            var xmlSerializer = new XmlSerializer(typeof(ImportOfficersDTO[]), new XmlRootAttribute("Officers"));
            var textRead = new StringReader(xmlString);
            var OfficersImortDtos = (ImportOfficersDTO[])xmlSerializer.Deserialize(textRead);

            foreach (var officerPrisoner in OfficersImortDtos)
            {
                if (!IsValid(officerPrisoner))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var officer = new Officer
                {
                    FullName = officerPrisoner.Name,
                    Salary = officerPrisoner.Money,
                    DepartmentId = officerPrisoner.DepartmentId,
                    Position = Enum.Parse<Position>(officerPrisoner.Position),
                    Weapon = Enum.Parse<Weapon>(officerPrisoner.Weapon),
                    OfficerPrisoners = officerPrisoner.Prisoners.Select(x => new OfficerPrisoner /*Пълнене на мапинг таблица*/
                    {
                        PrisonerId = x.Id
                    })
                    .ToArray()
                };

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}