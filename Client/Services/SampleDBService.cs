
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Radzen;

namespace SamplePWA.Client
{
    public partial class SampleDBService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public SampleDBService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/SampleDB/");
        }


        public async System.Threading.Tasks.Task ExportCustomersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportCustomersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetCustomers(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Customer>> GetCustomers(Query query)
        {
            return await GetCustomers(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Customer>> GetCustomers(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Customers");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetCustomers(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Customer>>(response);
        }

        partial void OnCreateCustomer(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Customer> CreateCustomer(SamplePWA.Server.Models.SampleDB.Customer customer = default(SamplePWA.Server.Models.SampleDB.Customer))
        {
            var uri = new Uri(baseUri, $"Customers");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");

            OnCreateCustomer(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Customer>(response);
        }

        partial void OnDeleteCustomer(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteCustomer(int customerId = default(int))
        {
            var uri = new Uri(baseUri, $"Customers({customerId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteCustomer(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetCustomerByCustomerId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Customer> GetCustomerByCustomerId(string expand = default(string), int customerId = default(int))
        {
            var uri = new Uri(baseUri, $"Customers({customerId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetCustomerByCustomerId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Customer>(response);
        }

        partial void OnUpdateCustomer(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateCustomer(int customerId = default(int), SamplePWA.Server.Models.SampleDB.Customer customer = default(SamplePWA.Server.Models.SampleDB.Customer))
        {
            var uri = new Uri(baseUri, $"Customers({customerId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", customer.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");

            OnUpdateCustomer(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportEmployeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/employees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/employees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportEmployeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/employees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/employees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetEmployees(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Employee>> GetEmployees(Query query)
        {
            return await GetEmployees(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Employee>> GetEmployees(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Employees");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetEmployees(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Employee>>(response);
        }

        partial void OnCreateEmployee(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Employee> CreateEmployee(SamplePWA.Server.Models.SampleDB.Employee employee = default(SamplePWA.Server.Models.SampleDB.Employee))
        {
            var uri = new Uri(baseUri, $"Employees");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");

            OnCreateEmployee(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Employee>(response);
        }

        partial void OnDeleteEmployee(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteEmployee(int employeeId = default(int))
        {
            var uri = new Uri(baseUri, $"Employees({employeeId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteEmployee(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetEmployeeByEmployeeId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Employee> GetEmployeeByEmployeeId(string expand = default(string), int employeeId = default(int))
        {
            var uri = new Uri(baseUri, $"Employees({employeeId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetEmployeeByEmployeeId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Employee>(response);
        }

        partial void OnUpdateEmployee(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateEmployee(int employeeId = default(int), SamplePWA.Server.Models.SampleDB.Employee employee = default(SamplePWA.Server.Models.SampleDB.Employee))
        {
            var uri = new Uri(baseUri, $"Employees({employeeId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", employee.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");

            OnUpdateEmployee(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportPaymentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/payments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/payments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportPaymentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/payments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/payments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetPayments(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Payment>> GetPayments(Query query)
        {
            return await GetPayments(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Payment>> GetPayments(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Payments");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPayments(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Payment>>(response);
        }

        partial void OnCreatePayment(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Payment> CreatePayment(SamplePWA.Server.Models.SampleDB.Payment payment = default(SamplePWA.Server.Models.SampleDB.Payment))
        {
            var uri = new Uri(baseUri, $"Payments");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");

            OnCreatePayment(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Payment>(response);
        }

        partial void OnDeletePayment(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeletePayment(int paymentId = default(int))
        {
            var uri = new Uri(baseUri, $"Payments({paymentId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeletePayment(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetPaymentByPaymentId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Payment> GetPaymentByPaymentId(string expand = default(string), int paymentId = default(int))
        {
            var uri = new Uri(baseUri, $"Payments({paymentId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPaymentByPaymentId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Payment>(response);
        }

        partial void OnUpdatePayment(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdatePayment(int paymentId = default(int), SamplePWA.Server.Models.SampleDB.Payment payment = default(SamplePWA.Server.Models.SampleDB.Payment))
        {
            var uri = new Uri(baseUri, $"Payments({paymentId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", payment.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");

            OnUpdatePayment(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportProductsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportProductsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetProducts(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Product>> GetProducts(Query query)
        {
            return await GetProducts(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Product>> GetProducts(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Products");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetProducts(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Product>>(response);
        }

        partial void OnCreateProduct(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Product> CreateProduct(SamplePWA.Server.Models.SampleDB.Product product = default(SamplePWA.Server.Models.SampleDB.Product))
        {
            var uri = new Uri(baseUri, $"Products");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(product), Encoding.UTF8, "application/json");

            OnCreateProduct(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Product>(response);
        }

        partial void OnDeleteProduct(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteProduct(int productId = default(int))
        {
            var uri = new Uri(baseUri, $"Products({productId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteProduct(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetProductByProductId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Product> GetProductByProductId(string expand = default(string), int productId = default(int))
        {
            var uri = new Uri(baseUri, $"Products({productId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetProductByProductId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Product>(response);
        }

        partial void OnUpdateProduct(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateProduct(int productId = default(int), SamplePWA.Server.Models.SampleDB.Product product = default(SamplePWA.Server.Models.SampleDB.Product))
        {
            var uri = new Uri(baseUri, $"Products({productId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", product.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(product), Encoding.UTF8, "application/json");

            OnUpdateProduct(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportProductCategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/productcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/productcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportProductCategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/productcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/productcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetProductCategories(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.ProductCategory>> GetProductCategories(Query query)
        {
            return await GetProductCategories(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.ProductCategory>> GetProductCategories(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"ProductCategories");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetProductCategories(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.ProductCategory>>(response);
        }

        partial void OnCreateProductCategory(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.ProductCategory> CreateProductCategory(SamplePWA.Server.Models.SampleDB.ProductCategory productCategory = default(SamplePWA.Server.Models.SampleDB.ProductCategory))
        {
            var uri = new Uri(baseUri, $"ProductCategories");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(productCategory), Encoding.UTF8, "application/json");

            OnCreateProductCategory(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.ProductCategory>(response);
        }

        partial void OnDeleteProductCategory(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteProductCategory(int categoryId = default(int))
        {
            var uri = new Uri(baseUri, $"ProductCategories({categoryId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteProductCategory(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetProductCategoryByCategoryId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.ProductCategory> GetProductCategoryByCategoryId(string expand = default(string), int categoryId = default(int))
        {
            var uri = new Uri(baseUri, $"ProductCategories({categoryId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetProductCategoryByCategoryId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.ProductCategory>(response);
        }

        partial void OnUpdateProductCategory(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateProductCategory(int categoryId = default(int), SamplePWA.Server.Models.SampleDB.ProductCategory productCategory = default(SamplePWA.Server.Models.SampleDB.ProductCategory))
        {
            var uri = new Uri(baseUri, $"ProductCategories({categoryId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", productCategory.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(productCategory), Encoding.UTF8, "application/json");

            OnUpdateProductCategory(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportPurchaseOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportPurchaseOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetPurchaseOrders(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.PurchaseOrder>> GetPurchaseOrders(Query query)
        {
            return await GetPurchaseOrders(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.PurchaseOrder>> GetPurchaseOrders(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"PurchaseOrders");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPurchaseOrders(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.PurchaseOrder>>(response);
        }

        partial void OnCreatePurchaseOrder(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrder> CreatePurchaseOrder(SamplePWA.Server.Models.SampleDB.PurchaseOrder purchaseOrder = default(SamplePWA.Server.Models.SampleDB.PurchaseOrder))
        {
            var uri = new Uri(baseUri, $"PurchaseOrders");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(purchaseOrder), Encoding.UTF8, "application/json");

            OnCreatePurchaseOrder(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.PurchaseOrder>(response);
        }

        partial void OnDeletePurchaseOrder(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeletePurchaseOrder(int orderId = default(int))
        {
            var uri = new Uri(baseUri, $"PurchaseOrders({orderId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeletePurchaseOrder(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetPurchaseOrderByOrderId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrder> GetPurchaseOrderByOrderId(string expand = default(string), int orderId = default(int))
        {
            var uri = new Uri(baseUri, $"PurchaseOrders({orderId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPurchaseOrderByOrderId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.PurchaseOrder>(response);
        }

        partial void OnUpdatePurchaseOrder(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdatePurchaseOrder(int orderId = default(int), SamplePWA.Server.Models.SampleDB.PurchaseOrder purchaseOrder = default(SamplePWA.Server.Models.SampleDB.PurchaseOrder))
        {
            var uri = new Uri(baseUri, $"PurchaseOrders({orderId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", purchaseOrder.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(purchaseOrder), Encoding.UTF8, "application/json");

            OnUpdatePurchaseOrder(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportPurchaseOrderDetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorderdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorderdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportPurchaseOrderDetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorderdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorderdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetPurchaseOrderDetails(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>> GetPurchaseOrderDetails(Query query)
        {
            return await GetPurchaseOrderDetails(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>> GetPurchaseOrderDetails(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"PurchaseOrderDetails");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPurchaseOrderDetails(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>>(response);
        }

        partial void OnCreatePurchaseOrderDetail(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> CreatePurchaseOrderDetail(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail purchaseOrderDetail = default(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail))
        {
            var uri = new Uri(baseUri, $"PurchaseOrderDetails");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(purchaseOrderDetail), Encoding.UTF8, "application/json");

            OnCreatePurchaseOrderDetail(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>(response);
        }

        partial void OnDeletePurchaseOrderDetail(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeletePurchaseOrderDetail(int orderDetailId = default(int))
        {
            var uri = new Uri(baseUri, $"PurchaseOrderDetails({orderDetailId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeletePurchaseOrderDetail(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetPurchaseOrderDetailByOrderDetailId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> GetPurchaseOrderDetailByOrderDetailId(string expand = default(string), int orderDetailId = default(int))
        {
            var uri = new Uri(baseUri, $"PurchaseOrderDetails({orderDetailId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPurchaseOrderDetailByOrderDetailId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>(response);
        }

        partial void OnUpdatePurchaseOrderDetail(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdatePurchaseOrderDetail(int orderDetailId = default(int), SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail purchaseOrderDetail = default(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail))
        {
            var uri = new Uri(baseUri, $"PurchaseOrderDetails({orderDetailId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", purchaseOrderDetail.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(purchaseOrderDetail), Encoding.UTF8, "application/json");

            OnUpdatePurchaseOrderDetail(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportSalesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/sales/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/sales/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportSalesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/sales/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/sales/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetSales(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Sale>> GetSales(Query query)
        {
            return await GetSales(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Sale>> GetSales(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Sales");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSales(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Sale>>(response);
        }

        partial void OnCreateSale(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Sale> CreateSale(SamplePWA.Server.Models.SampleDB.Sale sale = default(SamplePWA.Server.Models.SampleDB.Sale))
        {
            var uri = new Uri(baseUri, $"Sales");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(sale), Encoding.UTF8, "application/json");

            OnCreateSale(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Sale>(response);
        }

        partial void OnDeleteSale(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteSale(int saleId = default(int))
        {
            var uri = new Uri(baseUri, $"Sales({saleId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteSale(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetSaleBySaleId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Sale> GetSaleBySaleId(string expand = default(string), int saleId = default(int))
        {
            var uri = new Uri(baseUri, $"Sales({saleId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSaleBySaleId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Sale>(response);
        }

        partial void OnUpdateSale(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateSale(int saleId = default(int), SamplePWA.Server.Models.SampleDB.Sale sale = default(SamplePWA.Server.Models.SampleDB.Sale))
        {
            var uri = new Uri(baseUri, $"Sales({saleId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", sale.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(sale), Encoding.UTF8, "application/json");

            OnUpdateSale(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportSalesDetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/salesdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/salesdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportSalesDetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/salesdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/salesdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetSalesDetails(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.SalesDetail>> GetSalesDetails(Query query)
        {
            return await GetSalesDetails(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.SalesDetail>> GetSalesDetails(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"SalesDetails");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSalesDetails(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.SalesDetail>>(response);
        }

        partial void OnCreateSalesDetail(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.SalesDetail> CreateSalesDetail(SamplePWA.Server.Models.SampleDB.SalesDetail salesDetail = default(SamplePWA.Server.Models.SampleDB.SalesDetail))
        {
            var uri = new Uri(baseUri, $"SalesDetails");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(salesDetail), Encoding.UTF8, "application/json");

            OnCreateSalesDetail(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.SalesDetail>(response);
        }

        partial void OnDeleteSalesDetail(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteSalesDetail(int detailId = default(int))
        {
            var uri = new Uri(baseUri, $"SalesDetails({detailId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteSalesDetail(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetSalesDetailByDetailId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.SalesDetail> GetSalesDetailByDetailId(string expand = default(string), int detailId = default(int))
        {
            var uri = new Uri(baseUri, $"SalesDetails({detailId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSalesDetailByDetailId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.SalesDetail>(response);
        }

        partial void OnUpdateSalesDetail(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateSalesDetail(int detailId = default(int), SamplePWA.Server.Models.SampleDB.SalesDetail salesDetail = default(SamplePWA.Server.Models.SampleDB.SalesDetail))
        {
            var uri = new Uri(baseUri, $"SalesDetails({detailId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", salesDetail.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(salesDetail), Encoding.UTF8, "application/json");

            OnUpdateSalesDetail(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportSuppliersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/suppliers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/suppliers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportSuppliersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/suppliers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/suppliers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetSuppliers(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Supplier>> GetSuppliers(Query query)
        {
            return await GetSuppliers(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Supplier>> GetSuppliers(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Suppliers");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSuppliers(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<SamplePWA.Server.Models.SampleDB.Supplier>>(response);
        }

        partial void OnCreateSupplier(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Supplier> CreateSupplier(SamplePWA.Server.Models.SampleDB.Supplier supplier = default(SamplePWA.Server.Models.SampleDB.Supplier))
        {
            var uri = new Uri(baseUri, $"Suppliers");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(supplier), Encoding.UTF8, "application/json");

            OnCreateSupplier(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Supplier>(response);
        }

        partial void OnDeleteSupplier(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteSupplier(int supplierId = default(int))
        {
            var uri = new Uri(baseUri, $"Suppliers({supplierId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteSupplier(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetSupplierBySupplierId(HttpRequestMessage requestMessage);

        public async Task<SamplePWA.Server.Models.SampleDB.Supplier> GetSupplierBySupplierId(string expand = default(string), int supplierId = default(int))
        {
            var uri = new Uri(baseUri, $"Suppliers({supplierId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSupplierBySupplierId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<SamplePWA.Server.Models.SampleDB.Supplier>(response);
        }

        partial void OnUpdateSupplier(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateSupplier(int supplierId = default(int), SamplePWA.Server.Models.SampleDB.Supplier supplier = default(SamplePWA.Server.Models.SampleDB.Supplier))
        {
            var uri = new Uri(baseUri, $"Suppliers({supplierId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", supplier.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(supplier), Encoding.UTF8, "application/json");

            OnUpdateSupplier(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}