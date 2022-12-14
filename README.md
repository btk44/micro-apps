# micro-apps
This will be a set of minimal applications that can be used to construct bigger projects. It is some kind of microservice structure where all apps are loosely coupled and can be used in any other project.

The architecture of each service is inspired by Clean Architecture (by Jason Taylor). It may seem overengireered but it is done to make the services more customizable for different use cases. Key agruments:
* code will be ready to be used not only for web api but also for service bus communication
* Api, Application, Domain, Infrastructure can be easily moved to separate projects (like in Clean Architecture)
* ... tbc
 
The project will contain:
* docker compose files for each service
* account service - for creating accounts and indentity providing
* user service - for managing users under one account
* to be continued

## :computer: Tech stack

All the services are done using C#, .Net, SQLServer, Docker. 
UI - if will be done here - will be made with Vue probably.

## :wrench: Environment setup (fedora)

### :small_blue_diamond: .NET SDK
To install .NET SDK run following command: 
```
sudo dnf install dotnet-sdk-6.0 
```
If you need more information visit: [Install the .NET SDK or the .NET Runtime](https://docs.microsoft.com/en-us/dotnet/core/install/linux-fedora)

Note that if you start with a clean environment you will probably need to install some VS Code extentions, like `Nuget Package Manager` and tools like `dotnet-ef`. You will be notified during setup / development in output window if somethink is missing. 

Links:
[Entity Framework Core tools reference - .NET Core CLI](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

### :small_blue_diamond: Docker Engine
<sub><b>Note:</b> other system? Check out installation guides here [Install Docker Engine](https://docs.docker.com/engine/install/)</sub>

To get docker follow below instructions (available here: [Install Docker Engine on Fedora](https://docs.docker.com/engine/install/fedora/)):
```
sudo dnf -y install dnf-plugins-core
sudo dnf config-manager --add-repo https://download.docker.com/linux/fedora/docker-ce.repo
```
Then run:
```
sudo dnf install docker-ce docker-ce-cli containerd.io docker-compose-plugin
```
To start docker:
```
sudo systemctl start docker
```
Docker should be up and running now. But to be able to use it without admin ( `sudo` ) permissions we need to add our user to docker group.
```
sudo usermod -aG docker YOUR_USERNAME (you can get it by running 'whoami' command)
```
Now you need to logout and login again to apply the group changes. Now we are good to go.

### :small_blue_diamond: Portainer (optional)
This is not really needed. This good if you want to have visual form of presenting and managing docker containers. 


To install Portainer run following commands (available here: [Install Portainer with Docker on Linux](https://docs.portainer.io/v/ce-2.9/start/install/server/docker/linux)):
```
docker volume create portainer_data
docker run -d -p 8000:8000 -p 9443:9443 --name portainer --restart=always -v /var/run/docker.sock:/var/run/docker.sock -v portainer_data:/data portainer/portainer-ce:latest
```
Now you can got to your web browser and enter:
```
https://localhost:9443
```
You will be asked to create admin user name and password. 
