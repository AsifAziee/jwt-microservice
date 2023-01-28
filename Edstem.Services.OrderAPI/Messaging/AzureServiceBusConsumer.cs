using System.Text;
using Azure.Messaging.ServiceBus;
using Edstem.Services.OrderAPI.Models;
using Edstem.Services.OrderAPI.Models.Dto;
using Edstem.Services.OrderAPI.Repository;
using Newtonsoft.Json;

namespace Edstem.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private ServiceBusProcessor _checkoutProcessor;
        private readonly ILogger<AzureServiceBusConsumer> _logger;

        public AzureServiceBusConsumer(IConfiguration configuration,IOrderHeaderRepository orderHeaderRepository, ILogger<AzureServiceBusConsumer> logger)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _logger = logger;

            var client = new ServiceBusClient(configuration["ServiceBusConnectionString"]);
            _checkoutProcessor = client.CreateProcessor(configuration["TopicName"], configuration["SubscriptionName"]);
        }

        public async Task Start()
        {
            _checkoutProcessor.ProcessMessageAsync += OnCheckOutMessageRecieved;
            _checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await _checkoutProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _checkoutProcessor.StopProcessingAsync();
            await _checkoutProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError("Error occured while processing message: {e}", args.Exception);
            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);


            var checkoutDto = JsonConvert.DeserializeObject<CheckOutDto>(body);
            if (checkoutDto != null)
            {
                var orderHeader = new OrderHeader()
                {
                    UserId = checkoutDto.UserId,
                    FirstName = checkoutDto.FirstName,
                    LastName = checkoutDto.LastName,
                    OrderTotal = checkoutDto.OrderTotal,
                    OrderTime = DateTime.SpecifyKind(DateTime.Now,DateTimeKind.Utc),
                    PickupDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    Email = "not provided",
                    Phone = "not provided",
                    OrderDetails = new List<OrderDetails>()
                };

                foreach (var detailList in checkoutDto.CartDetails)
                    {
                    OrderDetails orderDetails = new()
                    {
                        ProductId = detailList.ProductId,
                        Count = detailList.Count,
                    };
                    orderHeader.CartTotalItems += detailList.Count;
                    orderHeader.OrderDetails.Add(orderDetails);
                }

                await _orderHeaderRepository.AddOrder(orderHeader);

            }
            else
            {
                _logger.LogError("Failed to deserialize message body:{0}", body);
            }
        }
    }
}
