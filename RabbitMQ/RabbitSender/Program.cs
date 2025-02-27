using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();

// Access user and pass of RabbitMQ which is running in docker 
// doccker sender and receiver port is 5672

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

factory.ClientProvidedName = "Rabbit Sender App";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

// Make channel and Bind that channel 

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey);

// Send Simple message

byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello Mehedi");
channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

channel.Close();
cnn.Close();
