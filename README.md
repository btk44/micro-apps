# micro-apps
Set of micro apps that can be useful when building something bigger ;)

The project will contain:
* docker compose files for each service
* account service - for creating accounts and indentity providing
* user service - for managing users under one account
* to be continued

All the services are done using C#, .Net, SQLServer, Docker. 
UI - if will be done here - will be made with Vue probably.

## :wrench: Environment setup (fedora)
Note: other system? Check out installation guides here [Install Docker Engine](https://docs.docker.com/engine/install/)

To get docker follow below instructions (available here: [Install Docker Engine on Fedora](https://docs.docker.com/engine/install/fedora/):
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
sudo usermod -aG docker YOUR_USERNAME (you can get it by running 'whoami' command
```
Now you need to logout and login again to apply the group changes. Now we are good to go.
