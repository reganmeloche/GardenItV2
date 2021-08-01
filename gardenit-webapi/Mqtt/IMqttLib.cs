using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace gardenit_webapi.Mqtt
{
    public interface IMqttLib
    {
        Task Init(List<Guid> clientIds);
        Task PublishMessage(Guid clientId, string message);
        void SetHandler(Func<MqttApplicationMessageReceivedEventArgs, Task> handler);
    }
}