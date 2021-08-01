using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace gardenit_webapi.Mqtt
{
    public class MqttLib : IMqttLib
    {
        private readonly IMqttClientOptions _options;
        private readonly IMqttClient _client;
        private Func<MqttApplicationMessageReceivedEventArgs, Task> _handler;

        private readonly string PUBLISH_TOPIC = "web_call";
        private readonly string SUBSCRIBE_TOPIC = "device_call";

        public MqttLib(IOptions<MqttOptions> optionsAccessor) {
            var opts = optionsAccessor.Value;
            string clientId = Guid.NewGuid().ToString();

            _options = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(opts.Host, opts.Port)
                .WithCredentials(opts.User, opts.Password)
                //.WithTls()
                .WithCleanSession()
                .Build();

            _client = new MqttFactory().CreateMqttClient();
        }

        public async Task Init(List<Guid> plantIds)
        {
            await ConnectAsync();
            await Subscribe(plantIds);
        }

        public void SetHandler(Func<MqttApplicationMessageReceivedEventArgs, Task> handler) {
            _handler = handler;
        }

        public async Task PublishMessage(Guid clientId, string payload) {
            if (!_client.IsConnected) {
                await ConnectAsync();
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"{PUBLISH_TOPIC}/{clientId}")
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await _client.PublishAsync(message, CancellationToken.None);
        }

        private async Task ConnectAsync()
        {
            await _client.ConnectAsync(_options, CancellationToken.None);
        }

        private async Task Subscribe(List<Guid> subscribeIds) {
            foreach (var clientId in subscribeIds) {
                await _client.SubscribeAsync(
                     new MqttTopicFilterBuilder()
                        .WithTopic($"{SUBSCRIBE_TOPIC}/{clientId}")
                        .Build()
                );
            }

            _client.UseApplicationMessageReceivedHandler(_handler);
        }
    }
}