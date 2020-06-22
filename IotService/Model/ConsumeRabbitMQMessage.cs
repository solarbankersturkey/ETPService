using IotService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IotService.Model
{
    public class ConsumeRabbitMQMessage : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly ICumulativeRepository _cumulativeRepository;

        public ConsumeRabbitMQMessage(ILoggerFactory loggerFactory, ICumulativeRepository cumulativeRepository)
        {
            this._logger = loggerFactory.CreateLogger<ConsumeRabbitMQMessage>();
            _cumulativeRepository = cumulativeRepository;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "34.71.94.126" ,UserName="admin",Password="Slbtr*a*9"};            
            // create connection  
            _connection = factory.CreateConnection();
            // create channel  
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare("slbex", ExchangeType.Direct);
            _channel.QueueDeclare("slbqueue", false, false, false, null);
            _channel.QueueBind("slbqueue", "slbex", "slbtr", null);
            //_channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                // handle the received message  
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("slbqueue", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(dynamic content)
        {
            try
            {
                dynamic jsonData = JObject.Parse(content);
                DateTime dt = Convert.ToDateTime(jsonData.date);
                string pureData = jsonData.rst.ToString();
                pureData = pureData.Replace(" ", String.Empty);
                Cumulative c = new Cumulative()
                {
                    Type = jsonData.type,
                    DynamicData = pureData,
                    GatewayId = jsonData.gateway_id,
                    Date = dt
                };

                _cumulativeRepository.Create(c);

                _logger.LogInformation($"consumer received {content}");
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error {e.Message}");
            }
            
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
