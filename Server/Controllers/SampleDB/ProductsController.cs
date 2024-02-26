using System;
using System.Net;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SamplePWA.Server.Controllers.SampleDB
{
    [Route("odata/SampleDB/Products")]
    public partial class ProductsController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public ProductsController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.Product> GetProducts()
        {
            var items = this.context.Products.AsQueryable<SamplePWA.Server.Models.SampleDB.Product>();
            this.OnProductsRead(ref items);

            return items;
        }

        partial void OnProductsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Product> items);

        partial void OnProductGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.Product> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/Products(ProductID={ProductID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.Product> GetProduct(int key)
        {
            var items = this.context.Products.Where(i => i.ProductID == key);
            var result = SingleResult.Create(items);

            OnProductGet(ref result);

            return result;
        }
        partial void OnProductDeleted(SamplePWA.Server.Models.SampleDB.Product item);
        partial void OnAfterProductDeleted(SamplePWA.Server.Models.SampleDB.Product item);

        [HttpDelete("/odata/SampleDB/Products(ProductID={ProductID})")]
        public IActionResult DeleteProduct(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Products
                    .Where(i => i.ProductID == key)
                    .Include(i => i.PurchaseOrderDetails)
                    .Include(i => i.SalesDetails)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Product>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnProductDeleted(item);
                this.context.Products.Remove(item);
                this.context.SaveChanges();
                this.OnAfterProductDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnProductUpdated(SamplePWA.Server.Models.SampleDB.Product item);
        partial void OnAfterProductUpdated(SamplePWA.Server.Models.SampleDB.Product item);

        [HttpPut("/odata/SampleDB/Products(ProductID={ProductID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutProduct(int key, [FromBody]SamplePWA.Server.Models.SampleDB.Product item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Products
                    .Where(i => i.ProductID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Product>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnProductUpdated(item);
                this.context.Products.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Products.Where(i => i.ProductID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ProductCategory");
                this.OnAfterProductUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/Products(ProductID={ProductID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchProduct(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.Product> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Products
                    .Where(i => i.ProductID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Product>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnProductUpdated(item);
                this.context.Products.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Products.Where(i => i.ProductID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "ProductCategory");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnProductCreated(SamplePWA.Server.Models.SampleDB.Product item);
        partial void OnAfterProductCreated(SamplePWA.Server.Models.SampleDB.Product item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.Product item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnProductCreated(item);
                this.context.Products.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Products.Where(i => i.ProductID == item.ProductID);

                Request.QueryString = Request.QueryString.Add("$expand", "ProductCategory");

                this.OnAfterProductCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
