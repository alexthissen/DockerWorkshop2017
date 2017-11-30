# Lab 2 - Dockerizing a .NET Core application

During this lab you will take an existing application and port it to use Docker containers.

Goals for this lab:
- [Run existing application](#run)
- [Add Docker support to .NET Core application](#add)
- [Run and debug applications from containers](#debug)
- [Build container images](#build)
- [Running SQL Server in a Docker container](#sql)

## <a name="run"></a>Run existing application
We will start with running the existing ASP.NET Core application from Visual Studio. Make sure you have cloned the Git repository, or return to [Lab 0 - Getting Started](Lab0-GettingStarted.md) to clone it now if you do not have the sources. Switch to the `Start` branch.

> ##### Important
> Make sure you have switched to the `Start` branch to use the right .NET solution. If you are still on the `master` branch, you will use the completed solution. 

Open the solution `RetroGaming2017.sln` in Visual Studio. Take your time to navigate the code and familiarize yourself with the various projects in the solution. You should be able to identify these:
- `GamingWebApp`, an ASP.NET MVC Core frontend 
- `Leaderboard.WebAPI`, an ASP.NET Core Web API 

For now, the SQL Server for Linux container instance is providing the developer backend for data storage. This will be changed later on.

Right-click both the GamingWebApp and Leaderboard.WebAPI and start to debug a new instance.

First, navigate to the web site located at http://localhost:46560/. There should be no highscores listed yet. Notice what the operating system is that you are currently running on.

Next, navigate to the Web API endpoint at http://localhost:31741/swagger. Experiment with the GET and POST operations that are offered from the Swagger user interface. Try to retrieve the list of high scores, and add a new high score for one of the registered player names.

Make sure you know how this application is implemented. Set breakpoints if necessary and navigate the flow of the application for the home page.

## <a name="add"></a>Add Docker support 

Visual Studio 2017 offers tooling for adding support to run your application in a Docker container. You will first add container support to the Web API project. 

To get started you can right-click the Leaderboard.WebAPI project and select Add, Docker Support from the context menu. Choose Linux as the target operating system.

<img src="images/AddDockerSupportTargetOS.png" width="400" />

Observe the changes that Visual Studio makes to your solution.  

Most noticeably you will see that a new Docker Compose project has been added with the name ```docker-compose```. It is now your start project for the solution. 

Inspect the contents of the `docker-compose.yml` and `docker-compose.override.yml` files if you haven't already. Ensure that you understand the meaning of the various entries in the YAML files.

Repeat adding Docker support for the Web application project. More changes will be made to the YAML files.

Run your application again. Which projects are effectively started? If some project is not running, start it by choosing Start, Debug instance from the right-click context menu of the project. 

> Does the application still work?

Now that the projects are running from a Docker container, the application might not work anymore. If not, try and find out what might be causing the issue. 

> Some things to try:
> - Inspect the running and stopped containers
> - Try to reach the Web API from http://localhost:31741/swagger.
> - Debug the call from the web page to the API by stepping through the code.
> - Verify application settings for each of the projects. 

The Web API and application need a couple of changes to make them work again. Here is your list of work that you need to do:
1. Change the exposed port of the Web API container in the `docker-compose.override.yml` file to be 1337.
```
ports:
  - "1337:1337"
```
2. Change the hosting URL of the Web API to be http://leaderboard.webapi:1337. Changing the port to 1337 is not required for fixing, but it clearly distinguishes the hosting in IIS Express (port 31741) from hosting in a Docker container (port 1337) and avoids port collision on port 80. 
```
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://0.0.0.0:1337
```

> You will learn more on networking later on. For now, notice that the URL is not referring to `localhost` but `leaderboard.webapi` (the name of the Docker container service as defined in the `docker-compose.yml` file).

3. Change the hosting port for the Web application to be 8080 instead of 80.

4. Change the `LeaderboardWebApiBaseUrl` to point to the new endpoint of the Web API with the internal address `http://leaderboard.webapi:1337`. Choose the right place to make that change, considering that you are now running from Docker containers. 
  
> ##### Hint
> Changing the setting in the `appsettings.json` file will work and you could choose to do so for now. It does mean that the setting for running without container will not work anymore. So, what other place can you think of that might work? Use that instead if you know, or just change `appsettings.json`.

```
gamingwebapp:
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    - LeaderboardWebApiBaseUrl=http://leaderboard.webapi:1337
```

5. Change the IP address of the connection string in the application settings for the Web API to be your local IP address instead of `127.0.0.1`. This is a temporary fix.

## <a name="debug"></a>Debugging with Docker container instances
One of the nicest features of the Docker support in Visual Studio is the debugging support while running container instances. Check out how easy debugging is by stepping through the application like before.

Put a breakpoint at the first statement of the `Index` method in the `HomeController` in the `GamingWebApp` project. Add another breakpoint in the `Get` method of the LeaderboardController in the Web API project.
Run the application by pressing F5. You should be hitting the breakpoints and jump from one container instance to the other.

## <a name="build"></a>Building container images
Start a command prompt and use the Docker CLI to check which container instances are running at the moment. There should be three containers related to the application:
- SQL Server in `sqldocker`.
- Web application in `dockercompose<id>_leaderboard.gamingwebapp_1`.
- Web API in `dockercompose<id>_leaderboard.webapi_1`.

where `<id>` is a random unique integer value.

> ##### New container images
> Which new container images are on your system at the moment? Check your images list with the Docker CLI

Stop your application if necessary. Verify that any container instances of the Web application or Web API are actually stopped. If not, stop them by executing the following command for each of the container instances, except `sqldocker`:

```
docker kill <container-id>
```

> Remember that you can use the first unique part of the container ID or its name

Now, try and run the Web application image yourself. Start a container instance.
```
docker run -p 8080:80 -it --name webapp gamingwebapp:dev
```
Check whether the web application is working. 

You should find that it does not work. The console output of the attached container gives a hint on what might be wrong.
```
Did you mean to run dotnet SDK commands? Please install dotnet SDK from:
  http://go.microsoft.com/fwlink/?LinkID=798306&clcid=0x409
```

Your container image does not contain any of the binaries that make your ASP.NET Core Web application run. Visual Studio uses volume mapping to map the files on your file system into the running container, so it can detect any changes thereby allowing small edits during debug sessions.

Let's create the image from the CLI to understand what is happening. If necessary, terminate the container instance you started by pressing `Ctrl+C` (possibly two times). Change your current directory to the root of the Web application project. It should contain the `Dockerfile` file. Execute the following command: 
```
docker build -t gamingwebapp:dev .
```

You might get an error indicating 

```
COPY failed: stat /var/lib/docker/tmp/.../obj/Docker/publish: no such file or directory
```

This is caused by the way that Visual Studio creates Docker images for debugging. It assumes that there is a source environment variable or it will fall back to a folder `obj\Docker\publish`. This can be found in the instruction `COPY ${source:-obj/Docker/publish} .` in the Dockerfile, which expresses that.

Run the command 

```
dotnet publish -o obj\Docker\publish
```
to create a publish folder in the expected place.

You should be able to create the Docker image successfully now. Run the Docker build command like before and try to start the Docker image. If all is well, your container should be running. 

> ##### Fix any errors if necessary
> Do not dwell on this too long, as you will probably not do manual building of images in the future

> ##### Debug images from Visual Studio
> Remember that Visual Studio creates Debug images that do not work when run from the Docker CLI. 

## <a name="sql"></a>Running SQL Server in a Docker container

Now that your application is running two projects in Docker containers, you can also run SQL Server from a container. This is convenient for isolated development and testing purposes. It eliminates the need to install SQL Server locally.

You have already pulled the SQL Server image in the previous lab. Go ahead and add the definition for a container service in the `docker-compose.yml` file.

```
  sql.data:
    image: microsoft/mssql-server-linux
```

Remember that from the Docker CLI you used many environment variables to bootstrap the container instance. Go back to the previous lab to check what these are.

The new container service also requires these same environment variables. Add them to the `docker-compose.override.yml` file under an entry for sql.data.

```
  sql.data:
    environment:
      - SA_PASSWORD=Pass@word
      - MSSQL_PID=Developer
      - ACCEPT_EULA=Y
    ports:
      - "1433:1433"
```

> ##### Which additional changes are needed?
> Stop and think about any other changes that might be required to take into account that the database server is now also running in a container.

You will need to change the connection string for the Web API to reflect the new way of hosting of the database. Add a new environment variable for the connection string of the leaderboard.webapi service in the `docker-compose.override.yml` file:

```
- ConnectionStrings:LeaderboardContext=Server=sql.data;Database=LeaderboardNETCore;User Id=sa;Password=Pass@word;Trusted_Connection=False
```

> ##### Strange connection string or not? 
> There are at least two remarkable things in this connection string. Can you spot them and explain why? Don't worry if not, as we will look at this in the [Networking](Lab3-Networking.md) lab.
 
With this change, you should be able to run your application completely from containers. Make sure you have stopped any containers related to the application. Give it a try and fix any issues that occur. 

> ##### Asking for help
> Remember that you can ask your proctor for help. Also, working with fellow attendees is highly recommended, as it can be fun and might be faster. Of course, you are free to offer help when asked.

## Wrapup
In this lab you have added Docker support to run both of your projects from Docker containers as well as the SQL Server instance. You enhanced the Docker Compose file that describes the composition of your complete application. in the next lab you will improve the networking part of the composition.

Continue with [Lab 3 - Networking](Lab3-Networking.md).
