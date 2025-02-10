using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class ResponseResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? RecordId { get; set; }
        public dynamic Records { get; set; }
        public int RecordCount { get; set; }
        public List<ResponseError> Errors { get; set; }

        public static ResponseResult CreateErrorResult(dynamic payload)
        {
            var response = new ResponseResult
            {
                Success = false,
                Errors = new()
            };

            var code = Convert.ToString(payload[0]["errorCode"]);
            var msg = Convert.ToString(payload[0]["message"]);
            response.Errors.Add(new ResponseError(code, msg));
            return response;
        }

        public static ResponseResult CreateSuccessResult(dynamic payload)
        {
            if (payload["records"].Count == 0) { 
                return new ResponseResult
                {
                    Success = false,
                    RecordCount = Convert.ToInt32(payload["totalSize"])
                };
            }
            var response = new ResponseResult
            {
                Success = true,
                Records = payload["records"],
                RecordCount = Convert.ToInt32(payload["totalSize"])
            };

            return response;
        }
    }

    public record ResponseError(string code, string message) { }
}
