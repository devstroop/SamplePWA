using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplePWA.Server.Models.SampleDB
{
    [Table("PurchaseOrder", Schema = "dbo")]
    public partial class PurchaseOrder
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
        public int OrderID { get; set; }

        [ConcurrencyCheck]
        public int? SupplierID { get; set; }

        public Supplier Supplier { get; set; }

        [ConcurrencyCheck]
        public DateTime? OrderDate { get; set; }

        [ConcurrencyCheck]
        public decimal? TotalAmount { get; set; }

        [ConcurrencyCheck]
        public string Status { get; set; }

        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    }
}