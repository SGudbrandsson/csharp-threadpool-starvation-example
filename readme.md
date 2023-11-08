---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Diagnostic scenarios sample debug target"
urlFragment: "diagnostic-scenarios"
description: "A .NET Core sample with methods that trigger undesirable behaviors to diagnose."
---
# To get threadpool starvation

Build and run the application locally
```bash
dotnet build
dotnet run
```

To monitor the threadpool starvation, use dotnet-counters in a separate terminal and monitor the ThreadPool Queue Length increase
```bash
dotnet-counters monitor -n DiagnosticScenarios
```

Then in a different terminal, run [bombardier](https://github.com/codesenberg/bombardier) with 250 threads for 2 minutes
```bash
bombardier -c 250 -d 120s http://localhost:5000/api/diagscenario/tasksleepwait
```

To see if you can get health checks despite the threadpool starving, you should go to the health check endpoint
```bash
# Broken liveness check
curl -v http://localhost:5000/health/live
# Working liveness check .. hopefully
curl -v http://localhost:7000/live
```

# Diagnostic scenarios sample debug target

The sample debug target is a simple `webapi` application. The sample triggers undesirable behaviors for the [.NET Core diagnostics tutorials](https://docs.microsoft.com/dotnet/core/diagnostics/index#net-core-diagnostics-tutorials) to diagnose.

## Download the source

To get the code locally on your machine, click on '<> Code' in the top left corner of this page. This will take you to the root of the repo. Once at the root, clone the samples repo onto your local machine and navigate to samples/core/diagnostics/DiagnosticScenarios/.

## Build and run the target

After downloading the source, you can easily run the webapi using:

```dotnetcli
dotnet build
dotnet run
```

## Target methods

The target triggers undesirable behaviors when hitting specific URLs.

### Deadlock

```http
http://localhost:5000/api/diagscenario/deadlock
```

This method will cause the target to hang and accumulate many threads.

### High CPU usage

```http
http://localhost:5000/api/diagscenario/highcpu/{milliseconds}
```

The method will cause to target to heavily use the CPU for a duration specified by {milliseconds}.

### Memory leak

```http
http://localhost:5000/api/diagscenario/memleak/{kb}
```

This method will cause the target to leak memory (amount specified by {kb}).

### Memory usage spike

```http
http://localhost:5000/api/diagscenario/memspike/{seconds}
```

This method will cause intermittent memory spikes over the specified number of seconds. Memory will go from base line to spike and back to baseline several times.
