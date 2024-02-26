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
    public partial class AddPurchaseOrder
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
            purchaseOrder = new SamplePWA.Server.Models.SampleDB.PurchaseOrder();
        }
        protected bool errorVisible;
        protected SamplePWA.Server.Models.SampleDB.PurchaseOrder purchaseOrder;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.Supplier> suppliersForSupplierID;


        protected int suppliersForSupplierIDCount;
        protected SamplePWA.Server.Models.SampleDB.Supplier suppliersForSupplierIDValue;
        protected async Task suppliersForSupplierIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetSuppliers(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Name, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                suppliersForSupplierID = result.Value.AsODataEnumerable();
                suppliersForSupplierIDCount = result.Count;

                if (!object.Equals(purchaseOrder.SupplierID, null))
                {
                    var valueResult = await SampleDBService.GetSuppliers(filter: $"SupplierID eq {purchaseOrder.SupplierID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        suppliersForSupplierIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Supplier" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await SampleDBService.CreatePurchaseOrder(purchaseOrder);
                DialogService.Close(purchaseOrder);
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