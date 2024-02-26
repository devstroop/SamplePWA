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
    [Route("odata/SampleDB/PurchaseOrderDetails")]
    public partial class PurchaseOrderDetailsController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public PurchaseOrderDetailsController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> GetPurchaseOrderDetails()
        {
            var items = this.context.PurchaseOrderDetails.AsQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>();
            this.OnPurchaseOrderDetailsRead(ref items);

            return items;
        }

        partial void OnPurchaseOrderDetailsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> items);

        partial void OnPurchaseOrderDetailGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/PurchaseOrderDetails(OrderDetailID={OrderDetailID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> GetPurchaseOrderDetail(int key)
        {
            var items = this.context.PurchaseOrderDetails.Where(i => i.OrderDetailID == key);
            var result = SingleResult.Create(items);

            OnPurchaseOrderDetailGet(ref result);

            return result;
        }
        partial void OnPurchaseOrderDetailDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);

        [HttpDelete("/odata/SampleDB/PurchaseOrderDetails(OrderDetailID={OrderDetailID})")]
        public IActionResult DeletePurchaseOrderDetail(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.PurchaseOrderDetails
                    .Where(i => i.OrderDetailID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnPurchaseOrderDetailDeleted(item);
                this.context.PurchaseOrderDetails.Remove(item);
                this.context.SaveChanges();
                this.OnAfterPurchaseOrderDetailDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPurchaseOrderDetailUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);

        [HttpPut("/odata/SampleDB/PurchaseOrderDetails(OrderDetailID={OrderDetailID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutPurchaseOrderDetail(int key, [FromBody]SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.PurchaseOrderDetails
                    .Where(i => i.OrderDetailID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnPurchaseOrderDetailUpdated(item);
                this.context.PurchaseOrderDetails.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PurchaseOrderDetails.Where(i => i.OrderDetailID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "PurchaseOrder,Product");
                this.OnAfterPurchaseOrderDetailUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/PurchaseOrderDetails(OrderDetailID={OrderDetailID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchPurchaseOrderDetail(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.PurchaseOrderDetails
                    .Where(i => i.OrderDetailID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnPurchaseOrderDetailUpdated(item);
                this.context.PurchaseOrderDetails.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PurchaseOrderDetails.Where(i => i.OrderDetailID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "PurchaseOrder,Product");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPurchaseOrderDetailCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item)
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

                this.OnPurchaseOrderDetailCreated(item);
                this.context.PurchaseOrderDetails.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.PurchaseOrderDetails.Where(i => i.OrderDetailID == item.OrderDetailID);

                Request.QueryString = Request.QueryString.Add("$expand", "PurchaseOrder,Product");

                this.OnAfterPurchaseOrderDetailCreated(item);

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
