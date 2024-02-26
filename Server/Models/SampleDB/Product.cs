using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplePWA.Server.Models.SampleDB
{
    [Table("Product", Schema = "dbo")]
    public partial class Product
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
        public int ProductID { get; set; }

        [ConcurrencyCheck]
        public string Name { get; set; }

        [ConcurrencyCheck]
        public string Description { get; set; }

        [ConcurrencyCheck]
        public decimal? Price { get; set; }

        [ConcurrencyCheck]
        public int? QuantityInStock { get; set; }

        [ConcurrencyCheck]
        public int? CategoryID { get; set; }

        public ProductCategory ProductCategory { get; set; }

        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public ICollection<SalesDetail> SalesDetails { get; set; }

    }
}