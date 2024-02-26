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
    public partial class EditPayment
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
        public int PaymentID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            payment = await SampleDBService.GetPaymentByPaymentId(paymentId:PaymentID);
        }
        protected bool errorVisible;
        protected SamplePWA.Server.Models.SampleDB.Payment payment;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.Customer> customersForCustomerID;


        protected int customersForCustomerIDCount;
        protected SamplePWA.Server.Models.SampleDB.Customer customersForCustomerIDValue;
        protected async Task customersForCustomerIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetCustomers(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Name, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                customersForCustomerID = result.Value.AsODataEnumerable();
                customersForCustomerIDCount = result.Count;

                if (!object.Equals(payment.CustomerID, null))
                {
                    var valueResult = await SampleDBService.GetCustomers(filter: $"CustomerID eq {payment.CustomerID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        customersForCustomerIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Customer" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await SampleDBService.UpdatePayment(paymentId:PaymentID, payment);
                if (result.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                {
                     hasChanges = true;
                     canEdit = false;
                     return;
                }
                DialogService.Close(payment);
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

            payment = await SampleDBService.GetPaymentByPaymentId(paymentId:PaymentID);
        }
    }
}