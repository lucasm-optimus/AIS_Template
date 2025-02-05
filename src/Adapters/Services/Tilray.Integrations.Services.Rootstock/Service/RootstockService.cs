using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using Tilray.Integrations.Core.Application.Rootstock.Services;
using Tilray.Integrations.Core.Domain;
using Tilray.Integrations.Core.Domain.Aggregates.Customer;
using Tilray.Integrations.Core.Domain.Aggregates.Sales;
using Tilray.Integrations.Services.Rootstock.Service.Queries;
using Tilray.Integrations.Services.Rootstock.Startup;
using static Tilray.Integrations.Core.Domain.Constants.Ecom;

namespace Tilray.Integrations.Services.Rootstock.Service
{
    public class RootstockService(HttpClient client, RootstockSettings rootstockSettings, ILogger<RootstockService> logger) : IRootstockService
    {
        #region Constants

        private const string QueryUrl = "services/data/v52.0/query";
        private const string SObjectUrl = "services/data/v59.0/sobjects";

        #endregion

        #region Private methods

        private static async Task<string> GetErrorFromResponseMessage(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return $"{response.StatusCode} (Details: '{content}')";
        }

        private async Task<dynamic> PostRootstockDataAsync(string objectName, object obj)
        {
            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{SObjectUrl}/{objectName}", content);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                string errorMessage = await GetErrorFromResponseMessage(response);
                logger.LogError($"Failed to create Customer. Error: {errorMessage}.{JsonConvert.SerializeObject(obj)}");
                return null;
            }
        }

        private async Task<dynamic?> ExecuteQueryAsync(string query)
        {
            HttpResponseMessage response = await client.GetAsync($"{QueryUrl}?q={query}");
            if (response.IsSuccessStatusCode)
            {
                var payload = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return ((IEnumerable<object>)payload?["records"])?.Count() > 0 ? payload?["records"] : null;
            }
            else
            {
                string errorMessage = await GetErrorFromResponseMessage(response);
                logger.LogError($"Failed to fetch. Error: {errorMessage}");
                return null;
            }
        }

        #endregion

        #region Public methods

        public async Task<string?> CreateCustomer(RstkCustomer customer)
        {
            var payload = await PostRootstockDataAsync(Constants.Rootstock.TableNames.Customer, customer);
            return RstkCustomer.GetCreatedRowId(payload);
        }

        public async Task<RstkCustomerInfoResponse?> GetCustomerInfo(string sfAccountId)
        {
            var formattedQuery = string.Format(RootstockQueries.GetCustomerBySfAccountQuery, sfAccountId);
            var payload = await ExecuteQueryAsync(formattedQuery);
            return payload == null ? null : RstkCustomerInfoResponse.MapFromPayload(payload);
        }

        public async Task<string?> CreateCustomerAddress(RstkCustomerAddress customerAddress)
        {
            var payload = await PostRootstockDataAsync(Constants.Rootstock.TableNames.CustomerAddress, customerAddress);
            return RstkCustomerAddress.GetCreatedRowId(payload);
        }

        public async Task<int?> GetCustomerAddressNextSequence(string customerNo)
        {
            var formattedQuery = string.Format(RootstockQueries.GetMaxAddressSequenceByCustomerNo, customerNo);
            var payload = await ExecuteQueryAsync(formattedQuery);
            return RstkCustomerAddressInfoResponse.GetNextSequenceNumber(payload);
        }

        public async Task<RstkCustomerAddressInfoResponse> GetCustomerAddressInfo(string customerNo, string addressType)
        {
            //await string.Format(, customerNo)
            var payload = addressType switch
            {
                "ShipTo" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetShipToCustomerAddressInfoQuery, customerNo)),
                "BillTo" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetDefaultBillToCustomerAddressInfoQuery, customerNo)),
                "Acknowledgement" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetAcknowledgementCustomerAddressInfoQuery, customerNo)),
                "Installation" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetInstallationCustomerAddressInfoQuery, customerNo)),
                _ => throw new Exception("Invalid address type.")
            };

            return RstkCustomerAddressInfoResponse.MapFromPayload(payload);
        }

        public async Task<RstkCustomerAddressInfoResponse> GetCustomerAddressInfo(string customerNo, string address, string city, string state, string zip)
        {
            string query =
                "SELECT  ID, rstk__externalid__c, Name, External_Customer_Number__c  FROM rstk__socaddr__c " +
                $" WHERE rstk__socaddr_address1__c = '{address.Replace("'", "\\'")}' " +
                $" AND rstk__externalid__c LIKE '{customerNo}_%'" +
                (city != null ? " AND rstk__socaddr_city__c = '" + city.Replace("'", "\\'") + "'" : "") +
                (state != null ? " AND rstk__socaddr_state__c = '" + state + "'" : "") +
                (zip != null ? " AND rstk__socaddr_zip__c = '" + zip + "'" : "") +
                " AND rstk__socaddr_useasshipto__c = true";

            var payload = await ExecuteQueryAsync(query);
            return RstkCustomerAddressInfoResponse.MapFromPayload(payload);
        }

        public async Task<dynamic> CreateSalesOrder(RstkSalesOrder salesOrder)
        {
            return await PostRootstockDataAsync(Constants.Rootstock.TableNames.SalesOrder, salesOrder);
        }

        public async Task<dynamic> CreateSalesOrderLineItem(RstkSalesOrderLineItem salesOrderLineItem)
        {
            return await PostRootstockDataAsync(Constants.Rootstock.TableNames.SalesOrder, salesOrderLineItem);
        }

        public async Task CreatePrePayment(RstkPrePayment prePayment)
        {
            await PostRootstockDataAsync(Constants.Rootstock.TableNames.Prepayment, prePayment);
        }

        public async Task<bool> SalesOrderExists(string customerReferenceNumber)
        {
            var formattedQuery = string.Format(RootstockQueries.GetListofCustomerReferencesByCustomerReferences, customerReferenceNumber);
            var payload = await ExecuteQueryAsync(formattedQuery);
            return payload != null && ((IEnumerable<object>)payload).Count() > 0;
        }

        #endregion
    }
}
