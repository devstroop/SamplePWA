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
    [Route("odata/SampleDB/Employees")]
    public partial class EmployeesController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public EmployeesController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.Employee> GetEmployees()
        {
            var items = this.context.Employees.AsQueryable<SamplePWA.Server.Models.SampleDB.Employee>();
            this.OnEmployeesRead(ref items);

            return items;
        }

        partial void OnEmployeesRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Employee> items);

        partial void OnEmployeeGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.Employee> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/Employees(EmployeeID={EmployeeID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.Employee> GetEmployee(int key)
        {
            var items = this.context.Employees.Where(i => i.EmployeeID == key);
            var result = SingleResult.Create(items);

            OnEmployeeGet(ref result);

            return result;
        }
        partial void OnEmployeeDeleted(SamplePWA.Server.Models.SampleDB.Employee item);
        partial void OnAfterEmployeeDeleted(SamplePWA.Server.Models.SampleDB.Employee item);

        [HttpDelete("/odata/SampleDB/Employees(EmployeeID={EmployeeID})")]
        public IActionResult DeleteEmployee(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Employees
                    .Where(i => i.EmployeeID == key)
                    .Include(i => i.Sales)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Employee>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnEmployeeDeleted(item);
                this.context.Employees.Remove(item);
                this.context.SaveChanges();
                this.OnAfterEmployeeDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnEmployeeUpdated(SamplePWA.Server.Models.SampleDB.Employee item);
        partial void OnAfterEmployeeUpdated(SamplePWA.Server.Models.SampleDB.Employee item);

        [HttpPut("/odata/SampleDB/Employees(EmployeeID={EmployeeID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutEmployee(int key, [FromBody]SamplePWA.Server.Models.SampleDB.Employee item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Employees
                    .Where(i => i.EmployeeID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Employee>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnEmployeeUpdated(item);
                this.context.Employees.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Employees.Where(i => i.EmployeeID == key);
                
                this.OnAfterEmployeeUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/Employees(EmployeeID={EmployeeID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchEmployee(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.Employee> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Employees
                    .Where(i => i.EmployeeID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.Employee>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnEmployeeUpdated(item);
                this.context.Employees.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Employees.Where(i => i.EmployeeID == key);
                
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnEmployeeCreated(SamplePWA.Server.Models.SampleDB.Employee item);
        partial void OnAfterEmployeeCreated(SamplePWA.Server.Models.SampleDB.Employee item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.Employee item)
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

                this.OnEmployeeCreated(item);
                this.context.Employees.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Employees.Where(i => i.EmployeeID == item.EmployeeID);

                

                this.OnAfterEmployeeCreated(item);

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
