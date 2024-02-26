using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using SamplePWA.Server.Data;

namespace SamplePWA.Server
{
    public partial class SampleDBService
    {
        SampleDBContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly SampleDBContext context;
        private readonly NavigationManager navigationManager;

        public SampleDBService(SampleDBContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportCustomersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCustomersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCustomersRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Customer> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.Customer>> GetCustomers(Query query = null)
        {
            var items = Context.Customers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCustomersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCustomerGet(SamplePWA.Server.Models.SampleDB.Customer item);
        partial void OnGetCustomerByCustomerId(ref IQueryable<SamplePWA.Server.Models.SampleDB.Customer> items);


        public async Task<SamplePWA.Server.Models.SampleDB.Customer> GetCustomerByCustomerId(int customerid)
        {
            var items = Context.Customers
                              .AsNoTracking()
                              .Where(i => i.CustomerID == customerid);

 
            OnGetCustomerByCustomerId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCustomerGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCustomerCreated(SamplePWA.Server.Models.SampleDB.Customer item);
        partial void OnAfterCustomerCreated(SamplePWA.Server.Models.SampleDB.Customer item);

        public async Task<SamplePWA.Server.Models.SampleDB.Customer> CreateCustomer(SamplePWA.Server.Models.SampleDB.Customer customer)
        {
            OnCustomerCreated(customer);

            var existingItem = Context.Customers
                              .Where(i => i.CustomerID == customer.CustomerID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Customers.Add(customer);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(customer).State = EntityState.Detached;
                throw;
            }

            OnAfterCustomerCreated(customer);

            return customer;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.Customer> CancelCustomerChanges(SamplePWA.Server.Models.SampleDB.Customer item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCustomerUpdated(SamplePWA.Server.Models.SampleDB.Customer item);
        partial void OnAfterCustomerUpdated(SamplePWA.Server.Models.SampleDB.Customer item);

        public async Task<SamplePWA.Server.Models.SampleDB.Customer> UpdateCustomer(int customerid, SamplePWA.Server.Models.SampleDB.Customer customer)
        {
            OnCustomerUpdated(customer);

            var itemToUpdate = Context.Customers
                              .Where(i => i.CustomerID == customer.CustomerID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(customer);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCustomerUpdated(customer);

            return customer;
        }

        partial void OnCustomerDeleted(SamplePWA.Server.Models.SampleDB.Customer item);
        partial void OnAfterCustomerDeleted(SamplePWA.Server.Models.SampleDB.Customer item);

        public async Task<SamplePWA.Server.Models.SampleDB.Customer> DeleteCustomer(int customerid)
        {
            var itemToDelete = Context.Customers
                              .Where(i => i.CustomerID == customerid)
                              .Include(i => i.Payments)
                              .Include(i => i.Sales)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCustomerDeleted(itemToDelete);


            Context.Customers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCustomerDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportEmployeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/employees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/employees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEmployeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/employees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/employees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEmployeesRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Employee> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.Employee>> GetEmployees(Query query = null)
        {
            var items = Context.Employees.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnEmployeesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEmployeeGet(SamplePWA.Server.Models.SampleDB.Employee item);
        partial void OnGetEmployeeByEmployeeId(ref IQueryable<SamplePWA.Server.Models.SampleDB.Employee> items);


        public async Task<SamplePWA.Server.Models.SampleDB.Employee> GetEmployeeByEmployeeId(int employeeid)
        {
            var items = Context.Employees
                              .AsNoTracking()
                              .Where(i => i.EmployeeID == employeeid);

 
            OnGetEmployeeByEmployeeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEmployeeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEmployeeCreated(SamplePWA.Server.Models.SampleDB.Employee item);
        partial void OnAfterEmployeeCreated(SamplePWA.Server.Models.SampleDB.Employee item);

        public async Task<SamplePWA.Server.Models.SampleDB.Employee> CreateEmployee(SamplePWA.Server.Models.SampleDB.Employee employee)
        {
            OnEmployeeCreated(employee);

            var existingItem = Context.Employees
                              .Where(i => i.EmployeeID == employee.EmployeeID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Employees.Add(employee);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(employee).State = EntityState.Detached;
                throw;
            }

            OnAfterEmployeeCreated(employee);

            return employee;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.Employee> CancelEmployeeChanges(SamplePWA.Server.Models.SampleDB.Employee item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEmployeeUpdated(SamplePWA.Server.Models.SampleDB.Employee item);
        partial void OnAfterEmployeeUpdated(SamplePWA.Server.Models.SampleDB.Employee item);

        public async Task<SamplePWA.Server.Models.SampleDB.Employee> UpdateEmployee(int employeeid, SamplePWA.Server.Models.SampleDB.Employee employee)
        {
            OnEmployeeUpdated(employee);

            var itemToUpdate = Context.Employees
                              .Where(i => i.EmployeeID == employee.EmployeeID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(employee);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEmployeeUpdated(employee);

            return employee;
        }

        partial void OnEmployeeDeleted(SamplePWA.Server.Models.SampleDB.Employee item);
        partial void OnAfterEmployeeDeleted(SamplePWA.Server.Models.SampleDB.Employee item);

        public async Task<SamplePWA.Server.Models.SampleDB.Employee> DeleteEmployee(int employeeid)
        {
            var itemToDelete = Context.Employees
                              .Where(i => i.EmployeeID == employeeid)
                              .Include(i => i.Sales)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEmployeeDeleted(itemToDelete);


            Context.Employees.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEmployeeDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPaymentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/payments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/payments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPaymentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/payments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/payments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPaymentsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Payment> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.Payment>> GetPayments(Query query = null)
        {
            var items = Context.Payments.AsQueryable();

            items = items.Include(i => i.Customer);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPaymentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPaymentGet(SamplePWA.Server.Models.SampleDB.Payment item);
        partial void OnGetPaymentByPaymentId(ref IQueryable<SamplePWA.Server.Models.SampleDB.Payment> items);


        public async Task<SamplePWA.Server.Models.SampleDB.Payment> GetPaymentByPaymentId(int paymentid)
        {
            var items = Context.Payments
                              .AsNoTracking()
                              .Where(i => i.PaymentID == paymentid);

            items = items.Include(i => i.Customer);
 
            OnGetPaymentByPaymentId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPaymentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPaymentCreated(SamplePWA.Server.Models.SampleDB.Payment item);
        partial void OnAfterPaymentCreated(SamplePWA.Server.Models.SampleDB.Payment item);

        public async Task<SamplePWA.Server.Models.SampleDB.Payment> CreatePayment(SamplePWA.Server.Models.SampleDB.Payment payment)
        {
            OnPaymentCreated(payment);

            var existingItem = Context.Payments
                              .Where(i => i.PaymentID == payment.PaymentID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Payments.Add(payment);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(payment).State = EntityState.Detached;
                throw;
            }

            OnAfterPaymentCreated(payment);

            return payment;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.Payment> CancelPaymentChanges(SamplePWA.Server.Models.SampleDB.Payment item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPaymentUpdated(SamplePWA.Server.Models.SampleDB.Payment item);
        partial void OnAfterPaymentUpdated(SamplePWA.Server.Models.SampleDB.Payment item);

        public async Task<SamplePWA.Server.Models.SampleDB.Payment> UpdatePayment(int paymentid, SamplePWA.Server.Models.SampleDB.Payment payment)
        {
            OnPaymentUpdated(payment);

            var itemToUpdate = Context.Payments
                              .Where(i => i.PaymentID == payment.PaymentID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(payment);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPaymentUpdated(payment);

            return payment;
        }

        partial void OnPaymentDeleted(SamplePWA.Server.Models.SampleDB.Payment item);
        partial void OnAfterPaymentDeleted(SamplePWA.Server.Models.SampleDB.Payment item);

        public async Task<SamplePWA.Server.Models.SampleDB.Payment> DeletePayment(int paymentid)
        {
            var itemToDelete = Context.Payments
                              .Where(i => i.PaymentID == paymentid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPaymentDeleted(itemToDelete);


            Context.Payments.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPaymentDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Product> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.Product>> GetProducts(Query query = null)
        {
            var items = Context.Products.AsQueryable();

            items = items.Include(i => i.ProductCategory);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductGet(SamplePWA.Server.Models.SampleDB.Product item);
        partial void OnGetProductByProductId(ref IQueryable<SamplePWA.Server.Models.SampleDB.Product> items);


        public async Task<SamplePWA.Server.Models.SampleDB.Product> GetProductByProductId(int productid)
        {
            var items = Context.Products
                              .AsNoTracking()
                              .Where(i => i.ProductID == productid);

            items = items.Include(i => i.ProductCategory);
 
            OnGetProductByProductId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductCreated(SamplePWA.Server.Models.SampleDB.Product item);
        partial void OnAfterProductCreated(SamplePWA.Server.Models.SampleDB.Product item);

        public async Task<SamplePWA.Server.Models.SampleDB.Product> CreateProduct(SamplePWA.Server.Models.SampleDB.Product product)
        {
            OnProductCreated(product);

            var existingItem = Context.Products
                              .Where(i => i.ProductID == product.ProductID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Products.Add(product);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(product).State = EntityState.Detached;
                throw;
            }

            OnAfterProductCreated(product);

            return product;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.Product> CancelProductChanges(SamplePWA.Server.Models.SampleDB.Product item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductUpdated(SamplePWA.Server.Models.SampleDB.Product item);
        partial void OnAfterProductUpdated(SamplePWA.Server.Models.SampleDB.Product item);

        public async Task<SamplePWA.Server.Models.SampleDB.Product> UpdateProduct(int productid, SamplePWA.Server.Models.SampleDB.Product product)
        {
            OnProductUpdated(product);

            var itemToUpdate = Context.Products
                              .Where(i => i.ProductID == product.ProductID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(product);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductUpdated(product);

            return product;
        }

        partial void OnProductDeleted(SamplePWA.Server.Models.SampleDB.Product item);
        partial void OnAfterProductDeleted(SamplePWA.Server.Models.SampleDB.Product item);

        public async Task<SamplePWA.Server.Models.SampleDB.Product> DeleteProduct(int productid)
        {
            var itemToDelete = Context.Products
                              .Where(i => i.ProductID == productid)
                              .Include(i => i.PurchaseOrderDetails)
                              .Include(i => i.SalesDetails)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductDeleted(itemToDelete);


            Context.Products.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductCategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/productcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/productcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductCategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/productcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/productcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductCategoriesRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.ProductCategory> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.ProductCategory>> GetProductCategories(Query query = null)
        {
            var items = Context.ProductCategories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductCategoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductCategoryGet(SamplePWA.Server.Models.SampleDB.ProductCategory item);
        partial void OnGetProductCategoryByCategoryId(ref IQueryable<SamplePWA.Server.Models.SampleDB.ProductCategory> items);


        public async Task<SamplePWA.Server.Models.SampleDB.ProductCategory> GetProductCategoryByCategoryId(int categoryid)
        {
            var items = Context.ProductCategories
                              .AsNoTracking()
                              .Where(i => i.CategoryID == categoryid);

 
            OnGetProductCategoryByCategoryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductCategoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductCategoryCreated(SamplePWA.Server.Models.SampleDB.ProductCategory item);
        partial void OnAfterProductCategoryCreated(SamplePWA.Server.Models.SampleDB.ProductCategory item);

        public async Task<SamplePWA.Server.Models.SampleDB.ProductCategory> CreateProductCategory(SamplePWA.Server.Models.SampleDB.ProductCategory productcategory)
        {
            OnProductCategoryCreated(productcategory);

            var existingItem = Context.ProductCategories
                              .Where(i => i.CategoryID == productcategory.CategoryID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ProductCategories.Add(productcategory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(productcategory).State = EntityState.Detached;
                throw;
            }

            OnAfterProductCategoryCreated(productcategory);

            return productcategory;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.ProductCategory> CancelProductCategoryChanges(SamplePWA.Server.Models.SampleDB.ProductCategory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductCategoryUpdated(SamplePWA.Server.Models.SampleDB.ProductCategory item);
        partial void OnAfterProductCategoryUpdated(SamplePWA.Server.Models.SampleDB.ProductCategory item);

        public async Task<SamplePWA.Server.Models.SampleDB.ProductCategory> UpdateProductCategory(int categoryid, SamplePWA.Server.Models.SampleDB.ProductCategory productcategory)
        {
            OnProductCategoryUpdated(productcategory);

            var itemToUpdate = Context.ProductCategories
                              .Where(i => i.CategoryID == productcategory.CategoryID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(productcategory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductCategoryUpdated(productcategory);

            return productcategory;
        }

        partial void OnProductCategoryDeleted(SamplePWA.Server.Models.SampleDB.ProductCategory item);
        partial void OnAfterProductCategoryDeleted(SamplePWA.Server.Models.SampleDB.ProductCategory item);

        public async Task<SamplePWA.Server.Models.SampleDB.ProductCategory> DeleteProductCategory(int categoryid)
        {
            var itemToDelete = Context.ProductCategories
                              .Where(i => i.CategoryID == categoryid)
                              .Include(i => i.Products)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductCategoryDeleted(itemToDelete);


            Context.ProductCategories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductCategoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPurchaseOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPurchaseOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPurchaseOrdersRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrder> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrder>> GetPurchaseOrders(Query query = null)
        {
            var items = Context.PurchaseOrders.AsQueryable();

            items = items.Include(i => i.Supplier);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPurchaseOrdersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPurchaseOrderGet(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);
        partial void OnGetPurchaseOrderByOrderId(ref IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrder> items);


        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrder> GetPurchaseOrderByOrderId(int orderid)
        {
            var items = Context.PurchaseOrders
                              .AsNoTracking()
                              .Where(i => i.OrderID == orderid);

            items = items.Include(i => i.Supplier);
 
            OnGetPurchaseOrderByOrderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPurchaseOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPurchaseOrderCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);
        partial void OnAfterPurchaseOrderCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrder> CreatePurchaseOrder(SamplePWA.Server.Models.SampleDB.PurchaseOrder purchaseorder)
        {
            OnPurchaseOrderCreated(purchaseorder);

            var existingItem = Context.PurchaseOrders
                              .Where(i => i.OrderID == purchaseorder.OrderID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.PurchaseOrders.Add(purchaseorder);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(purchaseorder).State = EntityState.Detached;
                throw;
            }

            OnAfterPurchaseOrderCreated(purchaseorder);

            return purchaseorder;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrder> CancelPurchaseOrderChanges(SamplePWA.Server.Models.SampleDB.PurchaseOrder item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPurchaseOrderUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);
        partial void OnAfterPurchaseOrderUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrder> UpdatePurchaseOrder(int orderid, SamplePWA.Server.Models.SampleDB.PurchaseOrder purchaseorder)
        {
            OnPurchaseOrderUpdated(purchaseorder);

            var itemToUpdate = Context.PurchaseOrders
                              .Where(i => i.OrderID == purchaseorder.OrderID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(purchaseorder);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPurchaseOrderUpdated(purchaseorder);

            return purchaseorder;
        }

        partial void OnPurchaseOrderDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);
        partial void OnAfterPurchaseOrderDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrder item);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrder> DeletePurchaseOrder(int orderid)
        {
            var itemToDelete = Context.PurchaseOrders
                              .Where(i => i.OrderID == orderid)
                              .Include(i => i.PurchaseOrderDetails)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPurchaseOrderDeleted(itemToDelete);


            Context.PurchaseOrders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPurchaseOrderDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPurchaseOrderDetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorderdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorderdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPurchaseOrderDetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/purchaseorderdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/purchaseorderdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPurchaseOrderDetailsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail>> GetPurchaseOrderDetails(Query query = null)
        {
            var items = Context.PurchaseOrderDetails.AsQueryable();

            items = items.Include(i => i.PurchaseOrder);
            items = items.Include(i => i.Product);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPurchaseOrderDetailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPurchaseOrderDetailGet(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);
        partial void OnGetPurchaseOrderDetailByOrderDetailId(ref IQueryable<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> items);


        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> GetPurchaseOrderDetailByOrderDetailId(int orderdetailid)
        {
            var items = Context.PurchaseOrderDetails
                              .AsNoTracking()
                              .Where(i => i.OrderDetailID == orderdetailid);

            items = items.Include(i => i.PurchaseOrder);
            items = items.Include(i => i.Product);
 
            OnGetPurchaseOrderDetailByOrderDetailId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPurchaseOrderDetailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPurchaseOrderDetailCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailCreated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> CreatePurchaseOrderDetail(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail purchaseorderdetail)
        {
            OnPurchaseOrderDetailCreated(purchaseorderdetail);

            var existingItem = Context.PurchaseOrderDetails
                              .Where(i => i.OrderDetailID == purchaseorderdetail.OrderDetailID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.PurchaseOrderDetails.Add(purchaseorderdetail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(purchaseorderdetail).State = EntityState.Detached;
                throw;
            }

            OnAfterPurchaseOrderDetailCreated(purchaseorderdetail);

            return purchaseorderdetail;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> CancelPurchaseOrderDetailChanges(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPurchaseOrderDetailUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailUpdated(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> UpdatePurchaseOrderDetail(int orderdetailid, SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail purchaseorderdetail)
        {
            OnPurchaseOrderDetailUpdated(purchaseorderdetail);

            var itemToUpdate = Context.PurchaseOrderDetails
                              .Where(i => i.OrderDetailID == purchaseorderdetail.OrderDetailID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(purchaseorderdetail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPurchaseOrderDetailUpdated(purchaseorderdetail);

            return purchaseorderdetail;
        }

        partial void OnPurchaseOrderDetailDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);
        partial void OnAfterPurchaseOrderDetailDeleted(SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail item);

        public async Task<SamplePWA.Server.Models.SampleDB.PurchaseOrderDetail> DeletePurchaseOrderDetail(int orderdetailid)
        {
            var itemToDelete = Context.PurchaseOrderDetails
                              .Where(i => i.OrderDetailID == orderdetailid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPurchaseOrderDetailDeleted(itemToDelete);


            Context.PurchaseOrderDetails.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPurchaseOrderDetailDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSalesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/sales/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/sales/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSalesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/sales/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/sales/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSalesRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Sale> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.Sale>> GetSales(Query query = null)
        {
            var items = Context.Sales.AsQueryable();

            items = items.Include(i => i.Customer);
            items = items.Include(i => i.Employee);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSalesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSaleGet(SamplePWA.Server.Models.SampleDB.Sale item);
        partial void OnGetSaleBySaleId(ref IQueryable<SamplePWA.Server.Models.SampleDB.Sale> items);


        public async Task<SamplePWA.Server.Models.SampleDB.Sale> GetSaleBySaleId(int saleid)
        {
            var items = Context.Sales
                              .AsNoTracking()
                              .Where(i => i.SaleID == saleid);

            items = items.Include(i => i.Customer);
            items = items.Include(i => i.Employee);
 
            OnGetSaleBySaleId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSaleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSaleCreated(SamplePWA.Server.Models.SampleDB.Sale item);
        partial void OnAfterSaleCreated(SamplePWA.Server.Models.SampleDB.Sale item);

        public async Task<SamplePWA.Server.Models.SampleDB.Sale> CreateSale(SamplePWA.Server.Models.SampleDB.Sale sale)
        {
            OnSaleCreated(sale);

            var existingItem = Context.Sales
                              .Where(i => i.SaleID == sale.SaleID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Sales.Add(sale);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(sale).State = EntityState.Detached;
                throw;
            }

            OnAfterSaleCreated(sale);

            return sale;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.Sale> CancelSaleChanges(SamplePWA.Server.Models.SampleDB.Sale item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSaleUpdated(SamplePWA.Server.Models.SampleDB.Sale item);
        partial void OnAfterSaleUpdated(SamplePWA.Server.Models.SampleDB.Sale item);

        public async Task<SamplePWA.Server.Models.SampleDB.Sale> UpdateSale(int saleid, SamplePWA.Server.Models.SampleDB.Sale sale)
        {
            OnSaleUpdated(sale);

            var itemToUpdate = Context.Sales
                              .Where(i => i.SaleID == sale.SaleID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(sale);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSaleUpdated(sale);

            return sale;
        }

        partial void OnSaleDeleted(SamplePWA.Server.Models.SampleDB.Sale item);
        partial void OnAfterSaleDeleted(SamplePWA.Server.Models.SampleDB.Sale item);

        public async Task<SamplePWA.Server.Models.SampleDB.Sale> DeleteSale(int saleid)
        {
            var itemToDelete = Context.Sales
                              .Where(i => i.SaleID == saleid)
                              .Include(i => i.SalesDetails)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSaleDeleted(itemToDelete);


            Context.Sales.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSaleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSalesDetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/salesdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/salesdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSalesDetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/salesdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/salesdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSalesDetailsRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.SalesDetail> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.SalesDetail>> GetSalesDetails(Query query = null)
        {
            var items = Context.SalesDetails.AsQueryable();

            items = items.Include(i => i.Product);
            items = items.Include(i => i.Sale);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSalesDetailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSalesDetailGet(SamplePWA.Server.Models.SampleDB.SalesDetail item);
        partial void OnGetSalesDetailByDetailId(ref IQueryable<SamplePWA.Server.Models.SampleDB.SalesDetail> items);


        public async Task<SamplePWA.Server.Models.SampleDB.SalesDetail> GetSalesDetailByDetailId(int detailid)
        {
            var items = Context.SalesDetails
                              .AsNoTracking()
                              .Where(i => i.DetailID == detailid);

            items = items.Include(i => i.Product);
            items = items.Include(i => i.Sale);
 
            OnGetSalesDetailByDetailId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSalesDetailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSalesDetailCreated(SamplePWA.Server.Models.SampleDB.SalesDetail item);
        partial void OnAfterSalesDetailCreated(SamplePWA.Server.Models.SampleDB.SalesDetail item);

        public async Task<SamplePWA.Server.Models.SampleDB.SalesDetail> CreateSalesDetail(SamplePWA.Server.Models.SampleDB.SalesDetail salesdetail)
        {
            OnSalesDetailCreated(salesdetail);

            var existingItem = Context.SalesDetails
                              .Where(i => i.DetailID == salesdetail.DetailID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.SalesDetails.Add(salesdetail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(salesdetail).State = EntityState.Detached;
                throw;
            }

            OnAfterSalesDetailCreated(salesdetail);

            return salesdetail;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.SalesDetail> CancelSalesDetailChanges(SamplePWA.Server.Models.SampleDB.SalesDetail item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSalesDetailUpdated(SamplePWA.Server.Models.SampleDB.SalesDetail item);
        partial void OnAfterSalesDetailUpdated(SamplePWA.Server.Models.SampleDB.SalesDetail item);

        public async Task<SamplePWA.Server.Models.SampleDB.SalesDetail> UpdateSalesDetail(int detailid, SamplePWA.Server.Models.SampleDB.SalesDetail salesdetail)
        {
            OnSalesDetailUpdated(salesdetail);

            var itemToUpdate = Context.SalesDetails
                              .Where(i => i.DetailID == salesdetail.DetailID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(salesdetail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSalesDetailUpdated(salesdetail);

            return salesdetail;
        }

        partial void OnSalesDetailDeleted(SamplePWA.Server.Models.SampleDB.SalesDetail item);
        partial void OnAfterSalesDetailDeleted(SamplePWA.Server.Models.SampleDB.SalesDetail item);

        public async Task<SamplePWA.Server.Models.SampleDB.SalesDetail> DeleteSalesDetail(int detailid)
        {
            var itemToDelete = Context.SalesDetails
                              .Where(i => i.DetailID == detailid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSalesDetailDeleted(itemToDelete);


            Context.SalesDetails.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSalesDetailDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSuppliersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/suppliers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/suppliers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSuppliersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/sampledb/suppliers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/sampledb/suppliers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSuppliersRead(ref IQueryable<SamplePWA.Server.Models.SampleDB.Supplier> items);

        public async Task<IQueryable<SamplePWA.Server.Models.SampleDB.Supplier>> GetSuppliers(Query query = null)
        {
            var items = Context.Suppliers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSuppliersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSupplierGet(SamplePWA.Server.Models.SampleDB.Supplier item);
        partial void OnGetSupplierBySupplierId(ref IQueryable<SamplePWA.Server.Models.SampleDB.Supplier> items);


        public async Task<SamplePWA.Server.Models.SampleDB.Supplier> GetSupplierBySupplierId(int supplierid)
        {
            var items = Context.Suppliers
                              .AsNoTracking()
                              .Where(i => i.SupplierID == supplierid);

 
            OnGetSupplierBySupplierId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSupplierGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSupplierCreated(SamplePWA.Server.Models.SampleDB.Supplier item);
        partial void OnAfterSupplierCreated(SamplePWA.Server.Models.SampleDB.Supplier item);

        public async Task<SamplePWA.Server.Models.SampleDB.Supplier> CreateSupplier(SamplePWA.Server.Models.SampleDB.Supplier supplier)
        {
            OnSupplierCreated(supplier);

            var existingItem = Context.Suppliers
                              .Where(i => i.SupplierID == supplier.SupplierID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Suppliers.Add(supplier);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(supplier).State = EntityState.Detached;
                throw;
            }

            OnAfterSupplierCreated(supplier);

            return supplier;
        }

        public async Task<SamplePWA.Server.Models.SampleDB.Supplier> CancelSupplierChanges(SamplePWA.Server.Models.SampleDB.Supplier item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSupplierUpdated(SamplePWA.Server.Models.SampleDB.Supplier item);
        partial void OnAfterSupplierUpdated(SamplePWA.Server.Models.SampleDB.Supplier item);

        public async Task<SamplePWA.Server.Models.SampleDB.Supplier> UpdateSupplier(int supplierid, SamplePWA.Server.Models.SampleDB.Supplier supplier)
        {
            OnSupplierUpdated(supplier);

            var itemToUpdate = Context.Suppliers
                              .Where(i => i.SupplierID == supplier.SupplierID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(supplier);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSupplierUpdated(supplier);

            return supplier;
        }

        partial void OnSupplierDeleted(SamplePWA.Server.Models.SampleDB.Supplier item);
        partial void OnAfterSupplierDeleted(SamplePWA.Server.Models.SampleDB.Supplier item);

        public async Task<SamplePWA.Server.Models.SampleDB.Supplier> DeleteSupplier(int supplierid)
        {
            var itemToDelete = Context.Suppliers
                              .Where(i => i.SupplierID == supplierid)
                              .Include(i => i.PurchaseOrders)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSupplierDeleted(itemToDelete);


            Context.Suppliers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSupplierDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}