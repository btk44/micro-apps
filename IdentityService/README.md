## Identity Service 
The service will hold all users accounts data. For now it is only email and password.

### :small_blue_diamond: Docker container setup:
Create a custom bridge network for our apps to communicate with each other:
```
docker network create --driver=bridge services-network
```

Running docker compose file:
```
docker-compose up -d
```

### :small_blue_diamond: Docker swarm service/container setup:

Build image:
```
docker build -t identity-service-img -f Dockerfile .
```

Init swarm:
```
docker swarm init
```

Create secret for token private key (if does not exist):
```
printf "super secret pass" | docker secret create private_key -

- note that in Windows you'll have to create a file with private key and run
docker secret create private_key .\secret_file
```


Create network (if does not exist):
```
docker network create -d overlay services-net
```

Create docker container with secret access
```
docker service create --publish 7000:80 \
    --secret="private_key" \
    --network services-net \
    --name identity-service --hostname identity-service \
    identity-service-img
```

### :small_blue_diamond: Test the container:

Access service api:
```
http://localhost:7000/swagger/index.html
```
### :small_blue_diamond: How to run migrations

After changing entities or relations between them run command (from csproj directory):
```
dotnet ef migrations add YourMigrationChangeName --output-dir Infrastructure/Migrations 
```