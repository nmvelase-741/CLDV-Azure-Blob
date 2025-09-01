using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using AzureStorageDemo.Models;

namespace AzureStorageDemo.Services
{
    // Handles Customers and Products in Azure Tables.
    public class TableStorageService
    {
        private readonly TableClient _customers;
        private readonly TableClient _products;

        public TableStorageService(string connectionString, string customersTable, string productsTable)
        {
            var tableService = new TableServiceClient(connectionString);

            _customers = tableService.GetTableClient(customersTable);
            _customers.CreateIfNotExists();

            _products = tableService.GetTableClient(productsTable);
            _products.CreateIfNotExists();
        }

        public async Task AddCustomerAsync(CustomerEntity customer)
            => await _customers.AddEntityAsync(customer);

        public async Task<List<CustomerEntity>> GetCustomersAsync()
        {
            var results = new List<CustomerEntity>();
            await foreach (var e in _customers.QueryAsync<CustomerEntity>())
                results.Add(e);
            return results;
        }

        public async Task AddProductAsync(ProductEntity product)
            => await _products.AddEntityAsync(product);

        public async Task<List<ProductEntity>> GetProductsAsync()
        {
            var results = new List<ProductEntity>();
            await foreach (var e in _products.QueryAsync<ProductEntity>())
                results.Add(e);
            return results;
        }
    }
}
