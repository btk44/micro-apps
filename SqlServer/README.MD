## Sql Server container setup 

All apps will use one contenerized sql server. Data separation will be achieved with schemas. Each service will use different one.
To get SqlServer container up and running follow instructions here: [Run SQL Server Linux container images with Docker](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash).

Run following command to pull image:
```
docker pull mcr.microsoft.com/mssql/server:2022-latest
```

### :small_blue_diamond: Docker container setup:
Create a custom bridge network for our apps to communicate with each other:
```
docker network create --driver=bridge services-network
```

Then run container by executing command:
```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyStrongPassw0rd!" -p 9333:1433 --name sql-server --hostname sql-server --network services-network -d mcr.microsoft.com/mssql/server:2022-latest
```
or run docker compose file:
```
cd SqlServer/
docker-compose up -d
```

### :small_blue_diamond: Docker swarm service/container setup:

Init swarm:
```
docker swarm init
```

Create network (if does not exist):
```
docker network create -d overlay services-net
```

Create docker container:
```
docker service create -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyStrongPassw0rd!" \
   -p 9333:1433 --name sql-server --hostname sql-server \
   --network services-net \
   mcr.microsoft.com/mssql/server:2022-latest
```

### :small_blue_diamond: Test the container:

Now if there were no errors you should be able to connect to your container using this credentials:
```
server: localhost
port: 9333
user: sa
password: MyStrongPassw0rd!
```
