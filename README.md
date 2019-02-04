# Serpentine

[![Build status](https://ci.appveyor.com/api/projects/status/w905nel94n35xyc7?svg=true)](https://ci.appveyor.com/project/gamingrobot/serpentine)

IIS Instrumentation Experiment

## Installation

### Single Application

### Global

## Features

- Measures Request/Handler Time
- Measures Response Size
- Adds metrics to html pages
- Add metrics to headers
- Supports [Server-Timing](https://www.w3.org/TR/server-timing/) headers

## Limitations

- Metrics are on a per Worker Process basis (only an issue if you have more than one worker process per app pool)
- Only works on Integrated Pipeline

## Requirements

- IIS 7 or higher
- .NET Framework 4.6.2
