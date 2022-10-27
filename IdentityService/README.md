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
to do

### :small_blue_diamond: Test the container:

Access service api:
```
http://localhost:7000/swagger/index.html
```
