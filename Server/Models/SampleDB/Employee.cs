using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplePWA.Server.Models.SampleDB
{
    [Table("Employee", Schema = "dbo")]
    public partial class Employee
    {

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("@odata.etag")]
        public string ETag
        {
                get;
                set;
        }

        [Key]
        [Required]
        public int EmployeeID { get; set; }

        [ConcurrencyCheck]
        public string FirstName { get; set; }

        [ConcurrencyCheck]
        public string LastName { get; set; }

        [ConcurrencyCheck]
        public string Email { get; set; }

        [ConcurrencyCheck]
        public string Phone { get; set; }

        [ConcurrencyCheck]
        public string Address { get; set; }

        [ConcurrencyCheck]
        public string Department { get; set; }

        [ConcurrencyCheck]
        public string Position { get; set; }

        [ConcurrencyCheck]
        public decimal? Salary { get; set; }

        public ICollection<Sale> Sales { get; set; }

    }
}