## Sql Server container setup 

All apps will use one contenerized sql server. Data separation will be achieved with schemas. Each service will use different one.
To get SqlServer container up and running follow instructions here: [Run SQL Server Linux container images with Docker](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash).

Run following command to pull image:
```
docker pull mcr.microsoft.com/mssql/server:2022-latest
```
Then run container by executing command:
```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyStrongPassw0rd!" -p 1433:1433 --name sql-server --hostname sql-server --network services-network -d mcr.microsoft.com/mssql/server:2022-latest
```
or running docker compose file:
```
cd SqlServer/
docker-compose up -d
```

Now if there was no errors you should be able to connect to your container using this credentials:
```
server: localhost
port: 1433
user: sa
password: MyStrongPassw0rd!
```