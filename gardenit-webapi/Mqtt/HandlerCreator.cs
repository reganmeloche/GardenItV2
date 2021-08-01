using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using gardenit_webapi.Lib;
using MQTTnet;

namespace gardenit_webapi.Mqtt
{
    public static class HandlerCreator
    {
        public static Func<MqttApplicationMessageReceivedEventArgs, Task> Create(IServiceScopeFactory sf) {
            return (MqttApplicationMessageReceivedEventArgs e) => {
                try
                {
                    string topic = e.ApplicationMessage.Topic;
                    Console.WriteLine(topic);

                    if (string.IsNullOrWhiteSpace(topic) == false)
                    {
                        string clientId = topic.Split("/")[1];
                        byte[] buffer = e.ApplicationMessage.Payload;
                        string message = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        // Log somewhere

                        if (message[0] == 'M') {
                            var moistureValue = Convert.ToDouble(message.Split(',')[1]);
                            var moistureReading = new Lib.MoistureReading() {
                                ReadDate = DateTime.Now,
                                Value = moistureValue
                            };
                        
                            var scope = sf.CreateScope();

                            var moistureLib = scope.ServiceProvider.GetService<IMoistureLib>();
                            moistureLib.AddReading(Guid.Parse(clientId), moistureReading);
                        }

                        Console.WriteLine($"Topic: {topic}. Message Received: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
                return Task.CompletedTask;
            };
        }
    }
}