using Azure;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using Tilray.Integrations.Core.Application.Rootstock.Services;
using Tilray.Integrations.Core.Domain;
using Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock;
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

        private async Task<ResponseResult> PostRootstockDataAsync(string objectName, object obj)
        {
            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{SObjectUrl}/{objectName}", content);
            var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation($"[Rootstock Create] Successfully created entry in entity {objectName}.");
                return ResponseResult.CreateSuccessResult(responseContent);
            }
            else
            {
                string errorMessage = $"{response.StatusCode} (Details: '{responseContent}')";
                logger.LogError($"[Rootstock Create] Failed to create entry in entity {objectName}. Error: {errorMessage}.");
                return ResponseResult.CreateErrorResult(responseContent);
            }
        }

        private async Task<ResponseResult> ExecuteQueryAsync(string query)
        {
            HttpResponseMessage response = await client.GetAsync($"{QueryUrl}?q={query}");
            var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                return ResponseResult.CreateSuccessResult(responseContent);
            }
            else
            {
                string errorMessage = $"{response.StatusCode} (Details: '{responseContent}')"; ;
                logger.LogError($"Failed to fetch. Error: {errorMessage}");
                return ResponseResult.CreateErrorResult(responseContent);
            }
        }

        #endregion

        #region Public methods

        public async Task<ResponseResult> CreateCustomer(RstkCustomer customer)
        {
            return await PostRootstockDataAsync(Constants.Rootstock.TableNames.Customer, customer);
        }

        public async Task<RstkCustomerInfoResponse> GetCustomerInfo(string sfAccountId)
        {
            var formattedQuery = string.Format(RootstockQueries.GetCustomerBySfAccountQuery, sfAccountId);
            var responseResult = await ExecuteQueryAsync(formattedQuery);
            return RstkCustomerInfoResponse.MapFromPayload(responseResult.Records);
        }

        public async Task<ResponseResult> CreateCustomerAddress(RstkCustomerAddress customerAddress)
        {
            return await PostRootstockDataAsync(Constants.Rootstock.TableNames.CustomerAddress, customerAddress);
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
            var responseResult = addressType switch
            {
                "ShipTo" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetShipToCustomerAddressInfoQuery, customerNo)),
                "BillTo" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetDefaultBillToCustomerAddressInfoQuery, customerNo)),
                "Acknowledgement" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetAcknowledgementCustomerAddressInfoQuery, customerNo)),
                "Installation" => await ExecuteQueryAsync(string.Format(RootstockQueries.GetInstallationCustomerAddressInfoQuery, customerNo)),
                _ => throw new Exception("Invalid address type.")
            };

            return RstkCustomerAddressInfoResponse.MapFromPayload(responseResult.Records);
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

            var responseResult = await ExecuteQueryAsync(query);
            return RstkCustomerAddressInfoResponse.MapFromPayload(responseResult.Records);
        }

        public async Task<ResponseResult> CreateSalesOrder(RstkSalesOrder salesOrder)
        {
            return await PostRootstockDataAsync(Constants.Rootstock.TableNames.SalesOrder, salesOrder);
        }

        public async Task<ResponseResult> CreateSalesOrderLineItem(RstkSalesOrderLineItem salesOrderLineItem)
        {
            return await PostRootstockDataAsync(Constants.Rootstock.TableNames.SalesOrder, salesOrderLineItem);
        }

        public async Task<ResponseResult> CreatePrePayment(RstkPrePayment prePayment)
        {
            return await PostRootstockDataAsync(Constants.Rootstock.TableNames.Prepayment, prePayment);
        }

        public async Task<bool> SalesOrderExists(string customerReferenceNumber)
        {
            var formattedQuery = string.Format(RootstockQueries.GetListofCustomerReferencesByCustomerReferences, customerReferenceNumber);
            var responseResult = await ExecuteQueryAsync(formattedQuery);
            return responseResult.RecordCount > 0;
        }

        #endregion
    }
}
