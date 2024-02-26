using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using SamplePWA.Server.Data;

namespace SamplePWA.Server.Controllers
{
    public partial class ExportSampleDBController : ExportController
    {
        private readonly SampleDBContext context;
        private readonly SampleDBService service;

        public ExportSampleDBController(SampleDBContext context, SampleDBService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/SampleDB/customers/csv")]
        [HttpGet("/export/SampleDB/customers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCustomers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/customers/excel")]
        [HttpGet("/export/SampleDB/customers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCustomers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/employees/csv")]
        [HttpGet("/export/SampleDB/employees/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmployeesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEmployees(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/employees/excel")]
        [HttpGet("/export/SampleDB/employees/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmployeesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEmployees(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/payments/csv")]
        [HttpGet("/export/SampleDB/payments/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPaymentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPayments(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/payments/excel")]
        [HttpGet("/export/SampleDB/payments/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPaymentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPayments(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/products/csv")]
        [HttpGet("/export/SampleDB/products/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProducts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/products/excel")]
        [HttpGet("/export/SampleDB/products/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProducts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/productcategories/csv")]
        [HttpGet("/export/SampleDB/productcategories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductCategoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProductCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/productcategories/excel")]
        [HttpGet("/export/SampleDB/productcategories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductCategoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProductCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/purchaseorders/csv")]
        [HttpGet("/export/SampleDB/purchaseorders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPurchaseOrdersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPurchaseOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/purchaseorders/excel")]
        [HttpGet("/export/SampleDB/purchaseorders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPurchaseOrdersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPurchaseOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/purchaseorderdetails/csv")]
        [HttpGet("/export/SampleDB/purchaseorderdetails/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPurchaseOrderDetailsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPurchaseOrderDetails(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/purchaseorderdetails/excel")]
        [HttpGet("/export/SampleDB/purchaseorderdetails/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPurchaseOrderDetailsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPurchaseOrderDetails(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/sales/csv")]
        [HttpGet("/export/SampleDB/sales/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSalesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSales(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/sales/excel")]
        [HttpGet("/export/SampleDB/sales/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSalesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSales(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/salesdetails/csv")]
        [HttpGet("/export/SampleDB/salesdetails/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSalesDetailsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSalesDetails(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/salesdetails/excel")]
        [HttpGet("/export/SampleDB/salesdetails/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSalesDetailsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSalesDetails(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/suppliers/csv")]
        [HttpGet("/export/SampleDB/suppliers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSuppliersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSuppliers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/SampleDB/suppliers/excel")]
        [HttpGet("/export/SampleDB/suppliers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSuppliersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSuppliers(), Request.Query, false), fileName);
        }
    }
}
