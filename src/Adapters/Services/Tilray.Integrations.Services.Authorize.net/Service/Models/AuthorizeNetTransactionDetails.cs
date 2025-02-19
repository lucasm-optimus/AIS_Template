namespace Tilray.Integrations.Services.Authorize.net.Service.Models;

public class GetTransactionDetailsRequest
{
    public MerchantAuthentication MerchantAuthentication { get; set; }
    public string TransId { get; set; }
}

public class MerchantAuthentication
{
    public string Name { get; set; }
    public string TransactionKey { get; set; }
}

public class TransactionDetailsRequest
{
    public GetTransactionDetailsRequest GetTransactionDetailsRequest { get; set; }

    public static TransactionDetailsRequest Create(string apiLoginId, string transactionKey, string transactionId)
    {
        return new TransactionDetailsRequest
        {
            GetTransactionDetailsRequest = new GetTransactionDetailsRequest
            {
                MerchantAuthentication = new MerchantAuthentication
                {
                    Name = apiLoginId,
                    TransactionKey = transactionKey
                },
                TransId = transactionId
            }
        };
    }
}

public class TransactionDetailsResponse
{
    public GetTransactionDetailsResponse GetTransactionDetailsResponse { get; set; }
    public Messages Messages { get; set; }
}

public class GetTransactionDetailsResponse
{
    public TransactionDetailsMessages Messages { get; set; }
    public Transaction Transaction { get; set; }
}

public class TransactionDetailsMessages
{
    public string ResultCode { get; set; }
    public Message Message { get; set; }
}

public class Message
{
    public string Code { get; set; }
    public string Text { get; set; }
}

public class Transaction
{
    public string TransactionStatus { get; set; }
}

public class TransactionErrorResponse
{
    public Messages Messages { get; set; }
}

public class Messages
{
    public string ResultCode { get; set; }
    public List<Message> Message { get; set; }
}

public class CreateTransactionRequest
{
    public MerchantAuthentication MerchantAuthentication { get; set; }
    public TransactionRequest TransactionRequest { get; set; }
}

public class TransactionRequest
{
    public string TransactionType { get; set; } = "priorAuthCaptureTransaction";
    public decimal Amount { get; set; }
    public string RefTransId { get; set; }
}

public class TransactionCaptureRequest
{
    public CreateTransactionRequest CreateTransactionRequest { get; set; }

    public static TransactionCaptureRequest Create(string apiLoginId, string transactionKey, string transactionId, decimal amount)
    {
        return new TransactionCaptureRequest
        {
            CreateTransactionRequest = new CreateTransactionRequest
            {
                MerchantAuthentication = new MerchantAuthentication
                {
                    Name = apiLoginId,
                    TransactionKey = transactionKey
                },
                TransactionRequest = new TransactionRequest
                {
                    Amount = amount,
                    RefTransId = transactionId
                }
            }
        };
    }
}

public class TransactionCaptureResponse
{
    public TransactionResponse TransactionResponse { get; set; }
}

public class TransactionResponse
{
    public string ResponseCode { get; set; }
    public Messages Messages { get; set; }
}
