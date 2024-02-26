using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplePWA.Server.Models.SampleDB
{
    [Table("Customer", Schema = "dbo")]
    public partial class Customer
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
        public int CustomerID { get; set; }

        [ConcurrencyCheck]
        public string Name { get; set; }

        [ConcurrencyCheck]
        public string Email { get; set; }

        [ConcurrencyCheck]
        public string Phone { get; set; }

        [ConcurrencyCheck]
        public string Address { get; set; }

        [ConcurrencyCheck]
        public decimal? CreditLimit { get; set; }

        [ConcurrencyCheck]
        public decimal? Balance { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public ICollection<Sale> Sales { get; set; }

    }
}