using Amazon;
using Amazon.SimpleNotificationService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Phone
{
    public static class SMSService
    {
        public static async Task<ServiceResponse> SendAsync(Message message)
        {
            var response = new ServiceResponse();
            if (message.IsValid())
            {
                var snsService = new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(AWSCredentials.AccessKey, AWSCredentials.SecretKey, RegionEndpoint.USEast1);

                var attributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "AWS.SNS.SMS.SenderID", new MessageAttributeValue() { StringValue = message.SenderId, DataType = "String" } },
                    { "AWS.SNS.SMS.SMSType", new MessageAttributeValue() { StringValue = message.Type.GetDescription(), DataType = "String" } }
                };

                var request = new Amazon.SimpleNotificationService.Model.PublishRequest
                {
                    PhoneNumber = $"+1{message.PhoneNumber}",
                    Message = message.TextMessage,
                    MessageAttributes = attributes
                };

                var result = await snsService.PublishAsync(request);
                if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    response.IsValid = true;
                else
                {
                    response.Errors.Add("SMS failed!");
                    response.IsValid = false;
                }
            }
            else
            {
                response.Errors = message.Errors;
                response.IsValid = false;
            }
            return response;
        }
    }
}
