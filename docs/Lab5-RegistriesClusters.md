# Lab 5 - Container registries and clusters

In this lab you are going to learn about registries and clusters. This includes pushing images to the registry and deploying compositions to a running cluster.

Goals for this lab:
- [Push images to Docker Hub registry](#push)
- [Connecting to your cluster](#connect)
- [Create and enhance cluster composition for Docker Swarm](#create)
- [Deploy images to cluster](#deploy)
- [Run and manage composition in cluster](#run)
- [(Optional) Switch to Azure SQL Database](#sql)

## <a name="push"></a>Pushing images to a registry

Docker registries are to Docker container images what NuGet feeds are to NuGet packages. They allow access to existing images that have been published by the owner. They can be private or publicly accessible.

Visit the website of [Docker Hub](https://hub.docker.com/) at https://hub.docker.com/ and sign up for a new account. If you have one already, you might want to create a new organization.

<img src="images/EmptyDockerHub.png" width="500"/>

Alternatively you can also create an Azure Container Registry which allows (multiple) private repositories inside your registry. 

From here, we will refer to both Docker Hub and Azure Contrainer Registry as the 'registry'. Your registry (or organization) name is needed in a moment.

Now that we have a registry, you should try to create working images for our application and push the images to the registry. First, create a `Release` build of the solution. Run the build to check whether it is working correctly. With as successful build run `docker images` and verify that you have new images that are not tagged as `:dev`. Output should be similar to this:

```
REPOSITORY                        TAG                 IMAGE ID            CREATED             SIZE
gamingwebapp                      latest              52ca01b894b7        26 minutes ago      302MB
leaderboard.webapi                latest              388e4e4f4abd        26 minutes ago      303MB
gamingwebapp                      dev                 838b58f6e96d        2 days ago          299MB
leaderboard.webapi                dev                 86c3b70e7efa        2 days ago          299MB
```

The images for the release build are tagged with `latest`. Make sure you understand why a `Debug` build creates images that are tagged `:dev`.

> ##### Hint
> You may want to look in the special folder `obj/Docker` at the solution level again.

The current images are not suitable to be pushed to the registry, since their name does not include the name of the registry you created a moment ago. That format is `registry/repository` with the repository representing the name of the container image as it is at the moment, e.g. `alexthissen/gamingwebapp`.

Tag the current images again to include the registry name. This will not create a new image, but it does appear as a separate entry in your local images list.

```
docker tag gamingwebapp:latest <registry>/gamingwebapp:latest
```
Make sure you replace the name `<registry>` with your registry name. Perform the tagging for the web API image as well. Verify that the images are tagged and have the same image ID as the ones without registry name.

Login to the registry. For example, for Docker Hub this will be:
```
docker login
```

When you have successfully logged in, push both of the images to the their respective repositories in the registry:

```
docker push <registry>/gamingwebapp:latest
```
Again remember to replace the registry name, with yours.
The output of the push will resemble something like this:
```
The push refers to repository [docker.io/<registry>/gamingwebapp]
a2a026d4db6f: Pushed
5170c7975ace: Pushed
e502c9e08b10: Mounted from microsoft/aspnetcore
8b2ae5b337fe: Mounted from microsoft/aspnetcore
587f57f69c02: Mounted from microsoft/aspnetcore
5c4bb5b2f630: Mounted from microsoft/aspnetcore
a75caa09eb1f: Mounted from microsoft/aspnetcore
latest: digest: sha256:05d2b2ceea30bbaa1fd0f37ac88d1185e66f055a29b85bf6438ce4658e379da6 size: 1791
```

Remove your local images for the web application release build from the Docker CLI by calling:
```
docker rmi <registry>/gamingwebapp:latest
docker rmi gamingwebapp:latest
docker rmi gamingwebapp:dev
```

Verify that the `gamingwebapp` images are no longer on your machine.
Visit your registry at https://hub.docker.com/r/<registry>/gamingwebapp/ to check whether the image is actually in the repository.

Then, try to pull the image from the registry with:
```
docker pull <registry>/gamingwebapp:latest
```
This process can be automated by a build and release pipeline. You will learn how to do that in a later lab.

## Connecting to your cluster

Connect to your cluster from a Git Shell or by using `PuTTY.exe`.
With PuTTY.exe

Open the [Azure Portal](https://portal.azure.com). Find the public IP address of the management endpoint for your cluster and the public DNS entry for the cluster. Examine `Deployments` of the resource group and follow the link to the deployment details. For the deployment called `Microsoft.Template`, you will find the `SSH TARGETS` output value. Copy the URL navigate to that address.

In the portal blade that opens, make note of the `Destination` IPv4 address that is listed and the `SERVICE` port number listed. Use the IP address prepended with `docker@` for the `Host Name` in the PuTTY Session dialog (top item in the tree) and the port number.
<img src="images/PuTTYSessions.png" width="500"/>

Navigate to the `Connection/SSH/Auth` node and specify your `sdp2017swarm_privatekey.ppk` file path in the dialog at the bottom text box.
Lastly, add a tunnel from port 2374 in the cluster to localhost:2374 on your local machine.

<img src="images/PuTTYTunnels.png" width="500"/>

> ##### Connection to your cluster (alternative)
> Another way to connect to the cluster is to run `Git Shell`, or any other command-line console that has support for SSH. In such a console, run the following command:
>```
>ssh -i "$env:workshop\sdp2017swarm_privatekey.ppk" -p 50000
>  -fNL localhost:2374:/var/run/docker.sock docker@<ipaddress>
>```
> replacing `<ipaddress>` with your IP address of the management endpoint. Again, make sure the $env:workshop variable is still set. You can also replace this with the full path to the `.ppk` file.
>
> Use whichever method you find convenient.

## Deploy your Docker composition to a cluster

With the connection to your cluster set up, you can deploy a composition to the cluster.
The `docker-compose.production.yml` file contains enough information for a simple deployment when combined with the `docker-compose.yml` file.

Start a PowerShell command prompt and set the `$env:workshop` variable again if necessary.
Similarly set the environment variable for `DOCKER_HOST` to be that of your created tunnel:
```
$env:DOCKER_HOST = "tcp://localhost:2374"  -- PowerShell or Git Shell

- or -
SET DOCKER_HOST=tcp://localhost:2374       -- Command prompt
export DOCKER_HOST=tcp://localhost:2374    -- Bash shell
```
You should now be able to run `docker images` and get a list of images unlike those on your local machine:
```
REPOSITORY                    TAG                 IMAGE ID            CREATED             SIZE
docker4x/guide-azure          17.09.0-ce-azure1   f58b225ecf09        7 weeks ago         288MB
docker4x/agent-azure          17.09.0-ce-azure1   5dff61a3c52e        7 weeks ago         136MB
docker4x/lookup-azure         17.09.0-ce-azure1   c8215c6ac345        7 weeks ago         237MB
docker4x/meta-azure           17.09.0-ce-azure1   814064ed2b16        7 weeks ago         25.5MB
docker4x/logger-azure         17.09.0-ce-azure1   2a44a38fbaf6        7 weeks ago         247MB
docker4x/init-azure           17.09.0-ce-azure1   191464444af2        7 weeks ago         297MB
docker4x/l4controller-azure   17.09.0-ce-azure1   7957a7a29c6a        7 weeks ago         16.7MB
```

Run a couple of commands to familiarize yourself with the cluster and its services and deployments (called 'stacks').
```
docker service ls
docker stack ls
```
These will probably both be empty.

## <a name="Create"></a>Create cluster composition for Docker Swarm

To deploy a Docker composition to your cluster you can use Docker Compose files with the following command:
```
docker stack deploy -c <docker-compose file> retrogamingstack
```

> ##### Valid service names
> When going to a cluster, the service names will be used to build a DNS name (internally). Make sure you remove the period (dot) from the service names `leaderboard.webapi` and `sql.data` and from all references with these names such as URLs, depends_on and connection strings.

Currently, the Docker CLI does not support using multiple Docker Compose YAML files. This means that you cannot use the base `docker-compose.yml` together with the production override. 
You need to use a workaround for now to be able to deploy a stack. Here are a couple of options you can try:

1. Create a separate `docker-compose.azure.yml` file that has a combination of the two compose files with relevant details. 
2. Use the Docker Compose `config` option to output the composed configuration to a stack file:
```
docker-compose -f docker-compose.yml -f docker-compose.production.yml config > docker-stack.azure.yml
```
3. (Requires experimental features on Docker daemon) Run the Docker Compose `bundle` action to bundle your compose files to a 'deployment bundle' in a `.dab` file:
```
docker-compose -f .\docker-compose.yml -f .\docker-compose.production.yml bundle -o docker-azure.dab
```

The first method is sure to give you the intended results, at the cost of duplication of deployment details. Your mileage may vary with the other two options.

> ##### Using containerized SQL Server in production
> It is not recommended to use SQL Server in a production scenario in this way. You will loose data, unless you take special measures, such as mounting volumes. 
> However, for now you will keep the SQL Service instance in a Docker container. 
>
> Make sure that you change the environment name to **Development** to provision the database `LeaderboardNetCore` and seed it using Entity Framework's `DbInitialize()`. 
>
> If you have time left, try the stretch exercise below to switch to an Azure SQL Database.

Whatever way you come up with the `docker-stack.azure.yml` file, execute the following to deploy your Docker stack to Azure:
```
cd $env:workshop
docker stack deploy -c docker-compose.azure.yml retrogamingstack
```
Keep iterating over the YAML file until you get it right. 
These commands might come in handy:
```
docker stack ps retrogamingstack            -- Lists running containers
docker stack services retrogamingstack      -- Show services in stack
docker stack rm retrogamingstack            -- Remove stack deployment
```

## <a name="sql"></a>(Optional) Switch to Azure SQL Database

If you have time left, you can remove the SQL Server container altogether and switch to Azure SQL Database.
The steps you need to do are:
- Provision an Azure SQL Server database called `LeaderboardNetCore`, and an Azure SQL Server if necessary.

![](images/AzureSQLDatabase.png)

- Add a firewall rule to the Azure SQL Server to allow traffic coming in from the cluster. You can find the IP address of the cluster in the external load balancer NAT rules. It should be different from the SSH IP address.
- Connect with SQL Server Management Studio or any other query tool to execute a couple of SQL scripts to the server and database:

```
CREATE DATABASE [LeaderboardNetCore]
GO

USE [LeaderboardNetCore]
GO
```
Create a SQL Create script for the two tables in your local SQL Server database in the Docker container, or use the provided `CreateDatabase.sql` file in the Git repository.

Next, run the following script on the database:

```
CREATE LOGIN sdp2017 WITH PASSWORD='abc123!@'

USE LeaderboardNETCore
GO

CREATE USER sdp2017
	FOR LOGIN sdp2017
	WITH DEFAULT_SCHEMA = dbo
GO
-- Add user to the database owner role
EXEC sp_addrolemember N'db_owner', N'sdp2017'
GO
```

- Change the connection string in the stack file `docker-compose.azure.yml` and remove the `sqldata` service from it, and any references to that service. It should resemble the following, with `sdp2017sql` replaced with your server name:
```
- ConnectionStrings:LeaderboardContext=Server=tcp:sdp2017sql.database.windows.net,1433;Initial Catalog=LeaderboardNetCore;Persist Security Info=False;User ID=sdp2017;Password=abc123!@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

- Set the `ASPNETCORE_ENVIRONMENT` for the `LeaderboardWebAPI` to `Production` again.

Try to deploy the stack again using Azure SQL Database now instead of the containerized version.

## Wrapup

In this lab you have created a composition that is able to deploy a stack of services to a cluster. Necessary changes to the environment variables were made and perhaps you even got to use an Azure SQL Database. 

Continue with [Lab 6 - Security](Lab6-Security.md).
