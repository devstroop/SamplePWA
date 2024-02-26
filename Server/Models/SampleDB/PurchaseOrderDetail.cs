using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplePWA.Server.Models.SampleDB
{
    [Table("PurchaseOrderDetails", Schema = "dbo")]
    public partial class PurchaseOrderDetail
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
        public int OrderDetailID { get; set; }

        [ConcurrencyCheck]
        public int? OrderID { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        [ConcurrencyCheck]
        public int? ProductID { get; set; }

        public Product Product { get; set; }

        [ConcurrencyCheck]
        public int? Quantity { get; set; }

        [ConcurrencyCheck]
        public decimal? UnitPrice { get; set; }

        [ConcurrencyCheck]
        public decimal? TotalPrice { get; set; }

    }
}