# Lab 8 - Exploration

Goals for this lab is to experiment with a number of improvements to the solution. You are free to investigate, explore and experiment.

## Some ideas

Here are a couple of ideas of improvements to the solution:

### Resilient proxy

The current proxy does not have any robust error handling or retry logic. You can improve the current implementation by adding support for retry and circuit breaker. The Polly NuGet package might come in handy here.