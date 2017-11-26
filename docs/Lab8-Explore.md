# Lab 8 - Free exploration

Goals for this lab is to experiment with a number of improvements to the solution. You are free to investigate, explore and experiment.

## Some ideas

Here are a couple of ideas of improvements to the solution:

### Resilient proxy

The current proxy does not have any robust error handling or retry logic. You can improve the current implementation by adding support for retry and circuit breaker. The Polly NuGet package might come in handy here.

### Instrumentation with Application Insights

Add instrumentation logic to your application to gain more insights in the operations. This way you can monitor:
- Key operational events in your .NET application 
- Container and cluster state
- Telemetry from within your application

### VSTS pipelines per service

Instead of releasing an entire composition you may want to selectively update services in your cluster.

Create a single release pipeline per service and use the SSH command task to update a service via:
```
docker service update leaderboardwebapi
```

Inspect the possible options with `docker service update --help` and select which ones you think are appropriate.

### Health endpoints and monitoring

Your containers are able to indicate their health via so-called health endpoints. Try to build such endpoints into your services and include them in the Docker composition file. 
