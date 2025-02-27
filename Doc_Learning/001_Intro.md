
## Introduction of RabbitMQ

- Traditional server 
- request -> Db call -> Response back 

<img src="img/001.png">

- In this case if web server is down or Database is down 
- for user eveythings has has running 
- if request is increase 4000/5000 what happend
- server may crash 

<img src="img/002.png">

- Server CPU or RAM Resources has to increase

### Messager Broker (Queue)

- Think email send by user Azure function receiving
- The pass to message Broker 
- message Broker User Get Done message 
- Resove this message  without knowing when message is resolve 
- Asyncronus way way comunication 
- Sender does not know resover 
- resover get message one by one and resove it 


<img src="img/003.png">
<img src="img/004.png">

- When server is down 
- message broker hold message long time 
- when ever the server is up and running 
- in Real world it is not just 100 message 1000 or more 
- is request increase we need to increase the server ...
- we can use Docker and Kubernetes the increase server 

<img src="img/005.png">

### one message broker is RabbitMQ

- it is a one type of Queue macanisome for sending messages and receiving message packets


- Start Docker RabbitMQ 

- `docker run -d --hostname rmq --name rabbit-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management`

- RabbitMQ Default User and Password  `guest`

<img src="img/006.png">

- No queue right now 

<img src="img/007.png">

- using C# using some queue element and read from queue 
- RabbitSender Project 
- Add Dependency of RabbitMQ

<img src="img/008.png">
<img src="img/009.png">

- Channel 
- Exchange
- Queue 

<img src="img/010.png">
<img src="img/011.png">

```C#
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

```

- Run the program and see on RabbitMQ

<img src="img/012.png">
<img src="img/013.png">

- only send message queue hold this message
- if any message receiver then get this message and Resolve it 


### Create Rabbit Receiver 1

- for Event listener always to listen for messages
- if console applicaiton enter key press to off the receiver event listener

```C# 
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
```



### All Code togeher with Large Message

- Sender side 

```C#
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

for(int i = 0; i < 100; i++)
{
    Console.WriteLine("Send Message " + i);
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Message #{i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
}

channel.Close();
cnn.Close();

```

- Receiver 1 Side code 

```C#
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
```

### Here two Receiver 1 & 2 

- we can multiple Receiver 
- we can send and receive multiple 
- At a time they can read and send message to RabbitMQ


<img src="img/014.png">

- 1 sender and 2 receiver

<img src="img/015.png">

- lot number of message is coming and slow prcessing
- see the status 

<img src="img/016.png">