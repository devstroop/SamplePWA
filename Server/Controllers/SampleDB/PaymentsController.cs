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
    [Route("odata/SampleDB/Payments")]
    public partial class PaymentsController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public PaymentsController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.Payment> GetPayments()
        {
            var items = this.context.Payments.AsQueryable<SamplePWA.Server.Models.SampleDB.Payment>();
            this.OnPaymentsRead(ref items);

            return items;
        }

        partial void OnPaymentsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Payment> items);

        partial void OnPaymentGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.Payment> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/Payments(PaymentID={PaymentID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.Payment> GetPayment(int key)
        {
            var items = this.context.Payments.Where(i => i.PaymentID == key);
            var result = SingleResult.Create(items);

            OnPaymentGet(ref result);

            return result;
        }
        partial void OnPaymentDeleted(SamplePWA.Server.Models.SampleDB.Payment item);
        partial void OnAfterPaymentDeleted(SamplePWA.Server.Models.SampleDB.Payment item);

        [HttpDelete("/odata/SampleDB/Payments(PaymentID={PaymentID})")]
        public IActionResult DeletePayment(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Payments
                    .Where(i => i.PaymentID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Payment>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnPaymentDeleted(item);
                this.context.Payments.Remove(item);
                this.context.SaveChanges();
                this.OnAfterPaymentDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPaymentUpdated(SamplePWA.Server.Models.SampleDB.Payment item);
        partial void OnAfterPaymentUpdated(SamplePWA.Server.Models.SampleDB.Payment item);

        [HttpPut("/odata/SampleDB/Payments(PaymentID={PaymentID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutPayment(int key, [FromBody]SamplePWA.Server.Models.SampleDB.Payment item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Payments
                    .Where(i => i.PaymentID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Payment>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnPaymentUpdated(item);
                this.context.Payments.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Payments.Where(i => i.PaymentID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Customer");
                this.OnAfterPaymentUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/Payments(PaymentID={PaymentID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchPayment(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.Payment> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Payments
                    .Where(i => i.PaymentID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Payment>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnPaymentUpdated(item);
                this.context.Payments.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Payments.Where(i => i.PaymentID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Customer");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPaymentCreated(SamplePWA.Server.Models.SampleDB.Payment item);
        partial void OnAfterPaymentCreated(SamplePWA.Server.Models.SampleDB.Payment item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.Payment item)
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

                this.OnPaymentCreated(item);
                this.context.Payments.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Payments.Where(i => i.PaymentID == item.PaymentID);

                Request.QueryString = Request.QueryString.Add("$expand", "Customer");

                this.OnAfterPaymentCreated(item);

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
