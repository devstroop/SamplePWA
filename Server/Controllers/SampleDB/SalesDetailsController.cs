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
    [Route("odata/SampleDB/SalesDetails")]
    public partial class SalesDetailsController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public SalesDetailsController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.SalesDetail> GetSalesDetails()
        {
            var items = this.context.SalesDetails.AsQueryable<SamplePWA.Server.Models.SampleDB.SalesDetail>();
            this.OnSalesDetailsRead(ref items);

            return items;
        }

        partial void OnSalesDetailsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.SalesDetail> items);

        partial void OnSalesDetailGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.SalesDetail> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/SalesDetails(DetailID={DetailID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.SalesDetail> GetSalesDetail(int key)
        {
            var items = this.context.SalesDetails.Where(i => i.DetailID == key);
            var result = SingleResult.Create(items);

            OnSalesDetailGet(ref result);

            return result;
        }
        partial void OnSalesDetailDeleted(SamplePWA.Server.Models.SampleDB.SalesDetail item);
        partial void OnAfterSalesDetailDeleted(SamplePWA.Server.Models.SampleDB.SalesDetail item);

        [HttpDelete("/odata/SampleDB/SalesDetails(DetailID={DetailID})")]
        public IActionResult DeleteSalesDetail(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.SalesDetails
                    .Where(i => i.DetailID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.SalesDetail>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSalesDetailDeleted(item);
                this.context.SalesDetails.Remove(item);
                this.context.SaveChanges();
                this.OnAfterSalesDetailDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSalesDetailUpdated(SamplePWA.Server.Models.SampleDB.SalesDetail item);
        partial void OnAfterSalesDetailUpdated(SamplePWA.Server.Models.SampleDB.SalesDetail item);

        [HttpPut("/odata/SampleDB/SalesDetails(DetailID={DetailID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutSalesDetail(int key, [FromBody]SamplePWA.Server.Models.SampleDB.SalesDetail item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.SalesDetails
                    .Where(i => i.DetailID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.SalesDetail>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSalesDetailUpdated(item);
                this.context.SalesDetails.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.SalesDetails.Where(i => i.DetailID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Product,Sale");
                this.OnAfterSalesDetailUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/SalesDetails(DetailID={DetailID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchSalesDetail(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.SalesDetail> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.SalesDetails
                    .Where(i => i.DetailID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.SalesDetail>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnSalesDetailUpdated(item);
                this.context.SalesDetails.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.SalesDetails.Where(i => i.DetailID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Product,Sale");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSalesDetailCreated(SamplePWA.Server.Models.SampleDB.SalesDetail item);
        partial void OnAfterSalesDetailCreated(SamplePWA.Server.Models.SampleDB.SalesDetail item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.SalesDetail item)
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

                this.OnSalesDetailCreated(item);
                this.context.SalesDetails.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.SalesDetails.Where(i => i.DetailID == item.DetailID);

                Request.QueryString = Request.QueryString.Add("$expand", "Product,Sale");

                this.OnAfterSalesDetailCreated(item);

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
