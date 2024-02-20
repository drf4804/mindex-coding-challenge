using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        // I added an EmployeeId field here as the P.K. since I wanted to separate Compensation and use a Compensation In-Memory DB
        [Key]
        public String EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public decimal Salary { get; set; }
        // I chose DateTime because I thought it would be the most seamless type to serialize/deserialize
        public DateTime EffectiveDate { get; set; }
    }
}
