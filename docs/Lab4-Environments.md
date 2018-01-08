# Lab 4 - Working with environments

In this lab you will learn how to create and run compositions. 

Goals for this lab:
- [Working with compositions and Docker Compose](#work)
- [Create compositions for different environments](#create)
- [Change implementation to work with environment variables](#change)

## <a name="work"></a>Working with compositions and Docker Compose

Compositions are essential to manage the many different combinations of containers, images, run-time details and environmental settings. Typically an application consists of multiple running containers. Managing each of these individually is both difficult and labor intensive. Moreover, it does not define the relationships and dependencies that exist.

Docker Compose is the tool of choice for this lab to manage compositions of containers. It allows you to use a command-line interface, similar to the Docker CLI, to interact with compositions defined in a `docker-compose.yml` file. There are other tools that allow the creation of compositions, such as the YAML files of Kubernetes. You will use Docker Compose in this lab.

To become familiar with Docker Compose you will first start a container based on a YAML file using `docker-compose.exe`. Take a look at the Visual Studio solution and examine the `docker-compose.ci.build.yml` file.

```
version: '3'

services:
  ci-build:
    image: microsoft/aspnetcore-build:1.0-2.0
    volumes:
      - .:/src
    working_dir: /src
    command: /bin/bash -c "dotnet restore ./RetroGaming2017.sln ...
```

The definitions in the compose file describe a service called `ci-build` that uses the image `microsoft/aspnetcore-build:1.0-2.0` and has a volume mapping to the root of the source code. The command starts a build in the working directory `src`. 

Start this composition by executing the command from the root of the Visual Studio solution where the Docker Compose YAML files are located:
```
docker-compose -f docker-compose.ci.build.yml up
```

The command will 'up' (meaning 'start') the composition and perform a build and publish of the projects in the solution `RetroGaming2017`. 

Later you will use this composition in your build pipeline to perform the build and publishing of the binaries required to create the container images of the solution.

## <a name="create"></a>Create compositions for different environments

One of the useful features of Docker Compose is the layering and cascading of multiple YAML compose files. With it you can introduce concepts such as base compositions, inheritance and overrides.

By default the Docker Compose tooling assumes that your main composition file is called `docker-compose.yml`. Executing `docker-compose <command>` will look for that particular file. Try to run a build that way.

```
docker-compose build
```
It is convenient when your `docker-compose.yml` file is able to build the container images of the composition. This assumes that the source code build has already been done. The build of the container images is typically defined in the `Dockerfile` in the root of the individual projects.

Make sure you understand the `docker-compose.yml` contents.

The Docker support in Visual Studio 2017 makes a similar assumption. It assumes that runtime details for compositions started from Visual Studio are defined in `docker-compose.override.yml`. Open that file which is located underneath the `docker-compose.yml` file in the Solution Explorer tree of Visual Studio.

The combination of the two aforementioned compose files is enough to start a composition. You will need to specify both files in the command in the correct order. 

```
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```

Starting the composition this way might work or fail, depending on whether you have run the ci-build composition before. Think about what happens in each of the cases. 

> ##### Hint
> You can explore the folder `RetroGaming2017\obj\Docker` and examine two additional compose files called `docker-compose.vs.debug.g.yml` and `docker-compose.vs.release.g.yml`.

Ideally, your override file for Visual Studio contains the service that are needed when running from the IDE on a development machine.

Take a moment to contemplate whether the `sql.data` service should be defined in the `docker-compose.yml` file or elsewhere. Remember that running SQL Server from a container is not recommended from a production scenario unless special measures have been taken. For local development purposes is it useful to have a SQL Server instance that loses its data on each start of the hosting container. 

Change the location of the definition to the override compose file. Merge it with the existing service. 

Enhance the override file by adding the dependencies of the web application on the web api, and the web api on the sql.data service. For each of the two dependent services `gamingwebapp` and `leaderboard.webapi` add a `depends_on` naming the dependency by service name. E.g.

```
    depends_on:
      - "leaderboard.webapi"
```

Next, you are going to create a similar compose override for a production situation. In essence the `docker-compose.override.yml` is the development environment override file by convention.

Create a new `docker-compose.production.yml` file in the `docker-compose` project and add the headers for a Docker Compose file:

```
version: '3'

services:
```

Unload the Docker Compose project and edit its `.dcproj` file. Add an item to the `ItemGroup` for the YAML files like so:
```
<ItemGroup>
    <None Include="docker-compose.production.yml">
      <DependentUpon>docker-compose.yml</DependentUpon>
    </None>
```
This will make the production file appear under the `docker-compose.yml` base file.

Edit the `docker-compose.production.yml` file to represent the environment variables and settings for the production situation. You should take a look at the port mappings and base URLs in particular. Use the existing override file as a reference.

Additionally, create a `appsettings.production.json` and change the settings to again reflect the production application settings.

> ##### Static vs. dynamic settings 
> Which settings can be considered dynamic or static? What happens with the various environment specific settings in the `appsettings.json` file? What type of settings can you store there? Change your file accordingly.


## <a name="change"></a>Working with environments in .NET Core

In this sample application the web application only has a single setting for an external Web API endpoint.
```
- LeaderboardWebApiBaseUrl=http://leaderboard.webapi:1337
```

Even so, you can formalize a group of related settings, regardless of their origin. This can be from one of the `appsettings.json` files, `docker-compose.override.yml` files or even environment variables. 

In the web application project create a new class called `WebAppSettings` and give it a single `string` property called  `LeaderboardWebApiBaseUrl`. In more complex scenarios this class would contain more properties for each of the settings.

Next, go to the `Startup` class and add a statement in the `ConfigureServices` method to load the web app settings from the configuration.
```
services.Configure<WebAppSettings>(Configuration);
```
This instructs the ASP.NET MVC Core dependency injection system to add an instance of the `WebAppSettings` class to the list of registered mappings. It allows you to inject the settings into any other object created by the DI system.

Open the `HomeController.cs` file and locate (or create) the constructor of the controller class. Change it to have two parameters, which will be injected:
```
public HomeController(IOptionsSnapshot<WebAppSettings> settings, ILoggerFactory loggerFactory)
```
Additionally, create a read-only field to hold the value of the injected `settings` parameter values. 

The last step is to use the values from the settings object at the appropriate place. Find the `Index` action method and use the value of the settings in the constructor of the proxy object.
```
public async Task<IActionResult> Index()
{
  LeaderboardProxy proxy = new LeaderboardProxy(settings.Value.LeaderboardWebApiBaseUrl, logger);
  ...
}
```
While you are looking at this, follow the code to the proxy implementation and give it another examination.

## Wrapup

In this lab you have examined the way environments can be used to distinguish various hosting situations for your Docker composition. It is important to know which settings must be changeable for different environemnts, as the Docker images that you build cannot be changed internally.

Continue with [Lab 5 - Registries and clusters](Lab5-RegistriesClusters.md).
