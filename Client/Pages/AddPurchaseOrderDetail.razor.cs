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
    public partial class AddPurchaseOrderDetail
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
            purchaseOrderDetail = new SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail();
        }
        protected bool errorVisible;
        protected SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail purchaseOrderDetail;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.PurchaseOrder> purchaseOrdersForOrderID;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.Product> productsForProductID;


        protected int purchaseOrdersForOrderIDCount;
        protected SamplePWA.Server.Models.SampleDB.PurchaseOrder purchaseOrdersForOrderIDValue;
        protected async Task purchaseOrdersForOrderIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetPurchaseOrders(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Status, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                purchaseOrdersForOrderID = result.Value.AsODataEnumerable();
                purchaseOrdersForOrderIDCount = result.Count;

                if (!object.Equals(purchaseOrderDetail.OrderID, null))
                {
                    var valueResult = await SampleDBService.GetPurchaseOrders(filter: $"OrderID eq {purchaseOrderDetail.OrderID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        purchaseOrdersForOrderIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load PurchaseOrder" });
            }
        }

        protected int productsForProductIDCount;
        protected SamplePWA.Server.Models.SampleDB.Product productsForProductIDValue;
        protected async Task productsForProductIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetProducts(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Name, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                productsForProductID = result.Value.AsODataEnumerable();
                productsForProductIDCount = result.Count;

                if (!object.Equals(purchaseOrderDetail.ProductID, null))
                {
                    var valueResult = await SampleDBService.GetProducts(filter: $"ProductID eq {purchaseOrderDetail.ProductID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        productsForProductIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Product" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await SampleDBService.CreatePurchaseOrderDetail(purchaseOrderDetail);
                DialogService.Close(purchaseOrderDetail);
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