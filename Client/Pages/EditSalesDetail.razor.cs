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
    public partial class EditSalesDetail
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

        [Parameter]
        public int DetailID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            salesDetail = await SampleDBService.GetSalesDetailByDetailId(detailId:DetailID);
        }
        protected bool errorVisible;
        protected SamplePWA.Server.Models.SampleDB.SalesDetail salesDetail;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.Sale> salesForSaleID;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.Product> productsForProductID;


        protected int salesForSaleIDCount;
        protected SamplePWA.Server.Models.SampleDB.Sale salesForSaleIDValue;
        protected async Task salesForSaleIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetSales(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Status, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                salesForSaleID = result.Value.AsODataEnumerable();
                salesForSaleIDCount = result.Count;

                if (!object.Equals(salesDetail.SaleID, null))
                {
                    var valueResult = await SampleDBService.GetSales(filter: $"SaleID eq {salesDetail.SaleID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        salesForSaleIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Sale" });
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

                if (!object.Equals(salesDetail.ProductID, null))
                {
                    var valueResult = await SampleDBService.GetProducts(filter: $"ProductID eq {salesDetail.ProductID}");
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
                var result = await SampleDBService.UpdateSalesDetail(detailId:DetailID, salesDetail);
                if (result.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                {
                     hasChanges = true;
                     canEdit = false;
                     return;
                }
                DialogService.Close(salesDetail);
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


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
            hasChanges = false;
            canEdit = true;

            salesDetail = await SampleDBService.GetSalesDetailByDetailId(detailId:DetailID);
        }
    }
}