using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.FirstConsole
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
            var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password,
            };

            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            var properties = model.CreateBasicProperties();

            //Caso o servidor do rabbit crashar, a mensagem continua salva.
            properties.Persistent = true;

            model.QueueDeclare(QueueName, true, false, false, null);
            Console.WriteLine("Queue Created");

            model.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
            Console.WriteLine("Exchange Created");

            //Eu crio uma rota (RoutingKey) na qual a minha fila ta interessada.
            model.QueueBind(QueueName, ExchangeName, "ChaveDaRota");
            Console.WriteLine("Exchange and queue bound");

            Console.WriteLine($"Conexão está aberta: { connection.IsOpen }");

            PublicarMensagem(model, properties, "Teste de mensagem entre Producer and Consumer");
            Console.WriteLine("Message sent");

            while (true)
            {
                Console.WriteLine("Digite alguma mensagem para ser enviada");
                var mensagem = Console.ReadLine();
                Console.WriteLine("Aperte ENTER para enviar ou Q para sair.");

                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Q)
                    break;

                if (key.Key == ConsoleKey.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(mensagem))
                    {
                        PublicarMensagem(model, properties, mensagem);
                        Console.WriteLine("Mensagem enviada");
                    }
                }
            }

            Console.ReadKey();
        }

        private static void PublicarMensagem(IModel model, IBasicProperties properties, string mensagem)
        {
            model.BasicPublish(ExchangeName, QueueName, properties, Encoding.Default.GetBytes(mensagem));
        }
    }
}
