using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsumerConsole
{
    class Program
    {
        //CONNECTION
        private const string HostName = "localhost";
        private const string UserName = "user";
        private const string Password = "password";

        //LOL
        private const string ExchangeName = "MyExchange";
        private const string QueueName = "MyQueue";

        static void Main(string[] args)
        {
            //Dar tempo que a mensagem seja enviada no projeto Producer (FirstConsole)
            Thread.Sleep(5000);

            IConnection connection = ConfigurarConexao();
            var model = connection.CreateModel();
            model.BasicQos(0, 1, false);

            //Consumer
            ConsumirMensagens(model);
            Console.ReadKey();
        }

        private static IConnection ConfigurarConexao()
        {
            var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password,
            };

            var connection = connectionFactory.CreateConnection();
            return connection;
        }

        private static void ConsumirMensagens(IModel model)
        {
            var consumer = new EventingBasicConsumer(model);

            consumer.Received += (sender, eventArgs) =>
            {
                var body = eventArgs.Body;
                Console.WriteLine($"Mensagem recebida: { Encoding.UTF8.GetString(body.ToArray()) }");
                model.BasicAck(eventArgs.DeliveryTag, false);
            };

            model.BasicConsume(QueueName, false, consumer);
        }
    }
}
