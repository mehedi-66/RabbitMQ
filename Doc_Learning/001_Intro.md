
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

<img src="img/005.png">