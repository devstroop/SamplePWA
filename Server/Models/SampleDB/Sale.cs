using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplePWA.Server.Models.SampleDB
{
    [Table("Sales", Schema = "dbo")]
    public partial class Sale
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
        public int SaleID { get; set; }

        [ConcurrencyCheck]
        public int? CustomerID { get; set; }

        public Customer Customer { get; set; }

        [ConcurrencyCheck]
        public int? EmployeeID { get; set; }

        public Employee Employee { get; set; }

        [ConcurrencyCheck]
        public DateTime? SaleDate { get; set; }

        [ConcurrencyCheck]
        public decimal? TotalAmount { get; set; }

        [ConcurrencyCheck]
        public string Status { get; set; }

        public ICollection<SalesDetail> SalesDetails { get; set; }

    }
}