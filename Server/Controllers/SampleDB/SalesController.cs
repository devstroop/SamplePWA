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
    [Route("odata/SampleDB/Sales")]
    public partial class SalesController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public SalesController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.Sale> GetSales()
        {
            var items = this.context.Sales.AsQueryable<SamplePWA.Server.Models.SampleDB.Sale>();
            this.OnSalesRead(ref items);

            return items;
        }

        partial void OnSalesRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Sale> items);

        partial void OnSaleGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.Sale> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/Sales(SaleID={SaleID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.Sale> GetSale(int key)
        {
            var items = this.context.Sales.Where(i => i.SaleID == key);
            var result = SingleResult.Create(items);

            OnSaleGet(ref result);

            return result;
        }
        partial void OnSaleDeleted(SamplePWA.Server.Models.SampleDB.Sale item);
        partial void OnAfterSaleDeleted(SamplePWA.Server.Models.SampleDB.Sale item);

        [HttpDelete("/odata/SampleDB/Sales(SaleID={SaleID})")]
        public IActionResult DeleteSale(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Sales
                    .Where(i => i.SaleID == key)
                    .Include(i => i.SalesDetails)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Sale>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSaleDeleted(item);
                this.context.Sales.Remove(item);
                this.context.SaveChanges();
                this.OnAfterSaleDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSaleUpdated(SamplePWA.Server.Models.SampleDB.Sale item);
        partial void OnAfterSaleUpdated(SamplePWA.Server.Models.SampleDB.Sale item);

        [HttpPut("/odata/SampleDB/Sales(SaleID={SaleID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutSale(int key, [FromBody]SamplePWA.Server.Models.SampleDB.Sale item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Sales
                    .Where(i => i.SaleID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Sale>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSaleUpdated(item);
                this.context.Sales.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Sales.Where(i => i.SaleID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Customer,Employee");
                this.OnAfterSaleUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/Sales(SaleID={SaleID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchSale(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.Sale> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Sales
                    .Where(i => i.SaleID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Sale>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnSaleUpdated(item);
                this.context.Sales.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Sales.Where(i => i.SaleID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Customer,Employee");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSaleCreated(SamplePWA.Server.Models.SampleDB.Sale item);
        partial void OnAfterSaleCreated(SamplePWA.Server.Models.SampleDB.Sale item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.Sale item)
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

                this.OnSaleCreated(item);
                this.context.Sales.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Sales.Where(i => i.SaleID == item.SaleID);

                Request.QueryString = Request.QueryString.Add("$expand", "Customer,Employee");

                this.OnAfterSaleCreated(item);

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
