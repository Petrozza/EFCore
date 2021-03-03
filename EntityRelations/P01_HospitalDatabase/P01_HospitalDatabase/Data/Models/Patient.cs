using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class Patient
    {

        public Patient()
        {
            Diagnoses = new HashSet<Diagnose>();
            Visitations = new HashSet<Visitation>();
            Prescriptions = new HashSet<PatientMedicament>();

        }
        public int PatientId { get; set; }

        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(250)]
        [Required]
        public string Address { get; set; }

        [Column(TypeName = "varchar(80)")]
        [Required]
        public string Email { get; set; }

        [Required]
        public bool HasInsurance { get; set; }

        public ICollection<Diagnose> Diagnoses { get; set; }// = new HashSet<Diagnose>();

        public ICollection<Visitation> Visitations { get; set; }// = new HashSet<Visitation>();

        public ICollection<PatientMedicament> Prescriptions { get; set; }// = new HashSet<PatientMedicament>();
    }
}
