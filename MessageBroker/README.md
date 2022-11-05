## Message broker

To run RabbitMQ in docker run:
```
docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Docker swarm version:
```
docker pull rabbitmq:3-management

docker service create -p 5672:5672 -p 15672:15672 \
    --network services-net \
    --name rabbitmq --hostname rabbitmq \
    rabbitmq:3-management
```
