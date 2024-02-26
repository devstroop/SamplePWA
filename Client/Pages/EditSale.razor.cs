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
    public partial class EditSale
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
        public int SaleID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            sale = await SampleDBService.GetSaleBySaleId(saleId:SaleID);
        }
        protected bool errorVisible;
        protected SamplePWA.Server.Models.SampleDB.Sale sale;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.Customer> customersForCustomerID;

        protected IEnumerable<SamplePWA.Server.Models.SampleDB.Employee> employeesForEmployeeID;


        protected int customersForCustomerIDCount;
        protected SamplePWA.Server.Models.SampleDB.Customer customersForCustomerIDValue;
        protected async Task customersForCustomerIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetCustomers(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Name, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                customersForCustomerID = result.Value.AsODataEnumerable();
                customersForCustomerIDCount = result.Count;

                if (!object.Equals(sale.CustomerID, null))
                {
                    var valueResult = await SampleDBService.GetCustomers(filter: $"CustomerID eq {sale.CustomerID}");
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

        protected int employeesForEmployeeIDCount;
        protected SamplePWA.Server.Models.SampleDB.Employee employeesForEmployeeIDValue;
        protected async Task employeesForEmployeeIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await SampleDBService.GetEmployees(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(FirstName, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                employeesForEmployeeID = result.Value.AsODataEnumerable();
                employeesForEmployeeIDCount = result.Count;

                if (!object.Equals(sale.EmployeeID, null))
                {
                    var valueResult = await SampleDBService.GetEmployees(filter: $"EmployeeID eq {sale.EmployeeID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        employeesForEmployeeIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Employee" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await SampleDBService.UpdateSale(saleId:SaleID, sale);
                if (result.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                {
                     hasChanges = true;
                     canEdit = false;
                     return;
                }
                DialogService.Close(sale);
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

            sale = await SampleDBService.GetSaleBySaleId(saleId:SaleID);
        }
    }
}