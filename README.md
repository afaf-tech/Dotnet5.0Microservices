## .Net Core 5 Microservices

This project is built to comprehensively learn about .Net Core 5;

## REQUIREMENTS

- .Net Core 5
- MongoDB

## Dependencies

- MongoDB

## Installation without Docker

- Install framework

```bash
$ dotnet build
```

## Debugging Tools

Up to you depends on IDE you are using.

## Development Guide

- Use Local Package Play.Common `dotnet add package Play.Common` when you want to build a new service. 
    It contains configuration and repository to connect with MongoDB;
- If the package Play.Common is not found register it using `dotnet add source {..\packages} -n PlayEconomy`  