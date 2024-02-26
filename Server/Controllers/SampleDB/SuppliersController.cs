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
    [Route("odata/SampleDB/Suppliers")]
    public partial class SuppliersController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public SuppliersController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.Supplier> GetSuppliers()
        {
            var items = this.context.Suppliers.AsQueryable<SamplePWA.Server.Models.SampleDB.Supplier>();
            this.OnSuppliersRead(ref items);

            return items;
        }

        partial void OnSuppliersRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Supplier> items);

        partial void OnSupplierGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.Supplier> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/Suppliers(SupplierID={SupplierID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.Supplier> GetSupplier(int key)
        {
            var items = this.context.Suppliers.Where(i => i.SupplierID == key);
            var result = SingleResult.Create(items);

            OnSupplierGet(ref result);

            return result;
        }
        partial void OnSupplierDeleted(SamplePWA.Server.Models.SampleDB.Supplier item);
        partial void OnAfterSupplierDeleted(SamplePWA.Server.Models.SampleDB.Supplier item);

        [HttpDelete("/odata/SampleDB/Suppliers(SupplierID={SupplierID})")]
        public IActionResult DeleteSupplier(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Suppliers
                    .Where(i => i.SupplierID == key)
                    .Include(i => i.PurchaseOrders)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Supplier>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSupplierDeleted(item);
                this.context.Suppliers.Remove(item);
                this.context.SaveChanges();
                this.OnAfterSupplierDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSupplierUpdated(SamplePWA.Server.Models.SampleDB.Supplier item);
        partial void OnAfterSupplierUpdated(SamplePWA.Server.Models.SampleDB.Supplier item);

        [HttpPut("/odata/SampleDB/Suppliers(SupplierID={SupplierID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutSupplier(int key, [FromBody]SamplePWA.Server.Models.SampleDB.Supplier item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Suppliers
                    .Where(i => i.SupplierID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Supplier>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSupplierUpdated(item);
                this.context.Suppliers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Suppliers.Where(i => i.SupplierID == key);
                
                this.OnAfterSupplierUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/Suppliers(SupplierID={SupplierID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchSupplier(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.Supplier> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Suppliers
                    .Where(i => i.SupplierID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Supplier>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnSupplierUpdated(item);
                this.context.Suppliers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Suppliers.Where(i => i.SupplierID == key);
                
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSupplierCreated(SamplePWA.Server.Models.SampleDB.Supplier item);
        partial void OnAfterSupplierCreated(SamplePWA.Server.Models.SampleDB.Supplier item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.Supplier item)
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

                this.OnSupplierCreated(item);
                this.context.Suppliers.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Suppliers.Where(i => i.SupplierID == item.SupplierID);

                

                this.OnAfterSupplierCreated(item);

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
