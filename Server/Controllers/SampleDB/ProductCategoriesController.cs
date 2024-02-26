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
    [Route("odata/SampleDB/ProductCategories")]
    public partial class ProductCategoriesController : ODataController
    {
        private SamplePWA.Server.Data.SampleDBContext context;

        public ProductCategoriesController(SamplePWA.Server.Data.SampleDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<SamplePWA.Server.Models.SampleDB.ProductCategory> GetProductCategories()
        {
            var items = this.context.ProductCategories.AsQueryable<SamplePWA.Server.Models.SampleDB.ProductCategory>();
            this.OnProductCategoriesRead(ref items);

            return items;
        }

        partial void OnProductCategoriesRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.ProductCategory> items);

        partial void OnProductCategoryGet(ref SingleResult<SamplePWA.Server.Models.SampleDB.ProductCategory> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/SampleDB/ProductCategories(CategoryID={CategoryID})")]
        public SingleResult<SamplePWA.Server.Models.SampleDB.ProductCategory> GetProductCategory(int key)
        {
            var items = this.context.ProductCategories.Where(i => i.CategoryID == key);
            var result = SingleResult.Create(items);

            OnProductCategoryGet(ref result);

            return result;
        }
        partial void OnProductCategoryDeleted(SamplePWA.Server.Models.SampleDB.ProductCategory item);
        partial void OnAfterProductCategoryDeleted(SamplePWA.Server.Models.SampleDB.ProductCategory item);

        [HttpDelete("/odata/SampleDB/ProductCategories(CategoryID={CategoryID})")]
        public IActionResult DeleteProductCategory(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.ProductCategories
                    .Where(i => i.CategoryID == key)
                    .Include(i => i.Products)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.ProductCategory>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnProductCategoryDeleted(item);
                this.context.ProductCategories.Remove(item);
                this.context.SaveChanges();
                this.OnAfterProductCategoryDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnProductCategoryUpdated(SamplePWA.Server.Models.SampleDB.ProductCategory item);
        partial void OnAfterProductCategoryUpdated(SamplePWA.Server.Models.SampleDB.ProductCategory item);

        [HttpPut("/odata/SampleDB/ProductCategories(CategoryID={CategoryID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutProductCategory(int key, [FromBody]SamplePWA.Server.Models.SampleDB.ProductCategory item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.ProductCategories
                    .Where(i => i.CategoryID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.ProductCategory>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnProductCategoryUpdated(item);
                this.context.ProductCategories.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ProductCategories.Where(i => i.CategoryID == key);
                
                this.OnAfterProductCategoryUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/SampleDB/ProductCategories(CategoryID={CategoryID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchProductCategory(int key, [FromBody]Delta<SamplePWA.Server.Models.SampleDB.ProductCategory> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.ProductCategories
                    .Where(i => i.CategoryID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<SamplePWA.Server.Models.SampleDB.ProductCategory>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnProductCategoryUpdated(item);
                this.context.ProductCategories.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ProductCategories.Where(i => i.CategoryID == key);
                
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnProductCategoryCreated(SamplePWA.Server.Models.SampleDB.ProductCategory item);
        partial void OnAfterProductCategoryCreated(SamplePWA.Server.Models.SampleDB.ProductCategory item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] SamplePWA.Server.Models.SampleDB.ProductCategory item)
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

                this.OnProductCategoryCreated(item);
                this.context.ProductCategories.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.ProductCategories.Where(i => i.CategoryID == item.CategoryID);

                

                this.OnAfterProductCategoryCreated(item);

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
