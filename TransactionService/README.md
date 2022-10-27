## Transaction Service 
The service will hold all users transactions and accounts data. 

### :small_blue_diamond: Docker container setup:
Create a custom bridge network for our apps to communicate with each other:
```
docker network create --driver=bridge services-network
```

Then run container by executing compose file:
```
docker-compose up -d
```

### :small_blue_diamond: Docker swarm service/container setup:

Build image:
```
docker build -t transaction-service-img -f Dockerfile .
```

Init swarm:
```
docker swarm init
```

Create secret for token private key (if not exists):
```
printf "super secret pass" | docker secret create private_key -
```

Create network (if not exists):
```
docker network create -d overlay services-net
```

Create docker container with secret access
```
docker service create --publish 7001:80 \
    --secret="private_key" \
    --network services-net \
    --name transaction-service --hostname transaction-service \
    transaction-service-img
```

### :small_blue_diamond: Test the container:

Access service api:
```
http://localhost:7001/swagger/index.html
```
