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
    [Route("odata/SampleDB/PurchaseOrders")]
    public partial class PurchaseOrdersController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public PurchaseOrdersController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.PurchaseOrder> GetPurchaseOrders()
        {
            var items = this.context.PurchaseOrders.AsQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrder>();
            this.OnPurchaseOrdersRead(ref items);

            return items;
        }

        partial void OnPurchaseOrdersRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrder> items);

        partial void OnPurchaseOrderGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.PurchaseOrder> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/PurchaseOrders(OrderID={OrderID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.PurchaseOrder> GetPurchaseOrder(int key)
        {
            var items = this.context.PurchaseOrders.Where(i => i.OrderID == key);
            var result = SingleResult.Create(items);

            OnPurchaseOrderGet(ref result);

            return result;
        }
        partial void OnPurchaseOrderDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);
        partial void OnAfterPurchaseOrderDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);

        [HttpDelete("/odata/SampleDB/PurchaseOrders(OrderID={OrderID})")]
        public IActionResult DeletePurchaseOrder(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.PurchaseOrders
                    .Where(i => i.OrderID == key)
                    .Include(i => i.PurchaseOrderDetails)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.PurchaseOrder>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnPurchaseOrderDeleted(item);
                this.context.PurchaseOrders.Remove(item);
                this.context.SaveChanges();
                this.OnAfterPurchaseOrderDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPurchaseOrderUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);
        partial void OnAfterPurchaseOrderUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);

        [HttpPut("/odata/SampleDB/PurchaseOrders(OrderID={OrderID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutPurchaseOrder(int key, [FromBody]SamplePWA.Server.Models.SampleDB.PurchaseOrder item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.PurchaseOrders
                    .Where(i => i.OrderID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.PurchaseOrder>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnPurchaseOrderUpdated(item);
                this.context.PurchaseOrders.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PurchaseOrders.Where(i => i.OrderID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Supplier");
                this.OnAfterPurchaseOrderUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/PurchaseOrders(OrderID={OrderID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchPurchaseOrder(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.PurchaseOrder> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.PurchaseOrders
                    .Where(i => i.OrderID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.PurchaseOrder>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnPurchaseOrderUpdated(item);
                this.context.PurchaseOrders.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PurchaseOrders.Where(i => i.OrderID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Supplier");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPurchaseOrderCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);
        partial void OnAfterPurchaseOrderCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.PurchaseOrder item)
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

                this.OnPurchaseOrderCreated(item);
                this.context.PurchaseOrders.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PurchaseOrders.Where(i => i.OrderID == item.OrderID);

                Request.QueryString = Request.QueryString.Add("$expand", "Supplier");

                this.OnAfterPurchaseOrderCreated(item);

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
