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
using Microsoft.AspNetCore.Authorization;

namespace SamplePWA.Server.Controllers.SampleDB
{
    [Route("odata/SampleDB/Customers")]
    public partial class CustomersController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public CustomersController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.Customer> GetCustomers()
        {
            var items = this.context.Customers.AsQueryable<SamplePWA.Server.Models.SampleDB.Customer>();
            this.OnCustomersRead(ref items);

            return items;
        }

        partial void OnCustomersRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Customer> items);

        partial void OnCustomerGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.Customer> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/Customers(CustomerID={CustomerID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.Customer> GetCustomer(int key)
        {
            var items = this.context.Customers.Where(i => i.CustomerID == key);
            var result = SingleResult.Create(items);

            OnCustomerGet(ref result);

            return result;
        }
        partial void OnCustomerDeleted(SamplePWA.Server.Models.SampleDB.Customer item);
        partial void OnAfterCustomerDeleted(SamplePWA.Server.Models.SampleDB.Customer item);

        [HttpDelete("/odata/SampleDB/Customers(CustomerID={CustomerID})")]
        public IActionResult DeleteCustomer(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Customers
                    .Where(i => i.CustomerID == key)
                    .Include(i => i.Payments)
                    .Include(i => i.Sales)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Customer>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnCustomerDeleted(item);
                this.context.Customers.Remove(item);
                this.context.SaveChanges();
                this.OnAfterCustomerDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnCustomerUpdated(SamplePWA.Server.Models.SampleDB.Customer item);
        partial void OnAfterCustomerUpdated(SamplePWA.Server.Models.SampleDB.Customer item);

        [HttpPut("/odata/SampleDB/Customers(CustomerID={CustomerID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutCustomer(int key, [FromBody]SamplePWA.Server.Models.SampleDB.Customer item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Customers
                    .Where(i => i.CustomerID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Customer>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnCustomerUpdated(item);
                this.context.Customers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Customers.Where(i => i.CustomerID == key);
                
                this.OnAfterCustomerUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/Customers(CustomerID={CustomerID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchCustomer(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.Customer> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Customers
                    .Where(i => i.CustomerID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Customer>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnCustomerUpdated(item);
                this.context.Customers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Customers.Where(i => i.CustomerID == key);
                
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnCustomerCreated(SamplePWA.Server.Models.SampleDB.Customer item);
        partial void OnAfterCustomerCreated(SamplePWA.Server.Models.SampleDB.Customer item);

        [Authorize("Admin")]
        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.Customer item)
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

                this.OnCustomerCreated(item);
                this.context.Customers.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Customers.Where(i => i.CustomerID == item.CustomerID);

                

                this.OnAfterCustomerCreated(item);

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
