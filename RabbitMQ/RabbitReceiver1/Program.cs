using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();

// Access user and pass of RabbitMQ which is running in docker 
// doccker sender and receiver port is 5672

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

factory.ClientProvidedName = "Rabbit Receiver1 App";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

// Make channel and Bind that channel 

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey);
channel.BasicQos(0, 1, false);
              // prefeachSize, prefeachCount, global
              // Qoudas 

// Received 
var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(1)).Wait();

    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Received Message: {message}");
    channel.BasicAck(args.DeliveryTag, false);
    // ack only 1 message 
    // withour ack RabbitMQ not delete the message from their 
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
cnn.Close();