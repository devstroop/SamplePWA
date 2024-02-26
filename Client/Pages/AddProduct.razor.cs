using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace SamplePWA.Client.Pages
{
    public partial class AddProduct
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public SampleDBService SampleDBService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            product = new SamplePWA.Server.Models.SampleDB.Product();
        }
        protected bool errorVisible;
        protected SamplePWA.Server.Models.SampleDB.Product product;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.ProductCategory> productCategoriesForCategoryID;


        protected int productCategoriesForCategoryIDCount;
        protected SamplePWA.Server.Models.SampleDB.ProductCategory productCategoriesForCategoryIDValue;
        protected async Task productCategoriesForCategoryIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetProductCategories(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Name, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                productCategoriesForCategoryID = result.Value.AsODataEnumerable();
                productCategoriesForCategoryIDCount = result.Count;

                if (!object.Equals(product.CategoryID, null))
                {
                    var valueResult = await SampleDBService.GetProductCategories(filter: $"CategoryID eq {product.CategoryID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        productCategoriesForCategoryIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load ProductCategory" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await SampleDBService.CreateProduct(product);
                DialogService.Close(product);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;

        [Inject]
        protected SecurityService Security { get; set; }
    }
}