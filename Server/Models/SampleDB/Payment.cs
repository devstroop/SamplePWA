using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplePWA.Server.Models.SampleDB
{
    [Table("Payment", Schema = "dbo")]
    public partial class Payment
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
        public int PaymentID { get; set; }

        [ConcurrencyCheck]
        public int? CustomerID { get; set; }

        public Customer Customer { get; set; }

        [ConcurrencyCheck]
        public decimal? Amount { get; set; }

        [ConcurrencyCheck]
        public DateTime? PaymentDate { get; set; }

        [ConcurrencyCheck]
        public string PaymentMethod { get; set; }

    }
}