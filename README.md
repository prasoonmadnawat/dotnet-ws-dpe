# Dotnet Workshop
This repository contains a set of projects that will help introduce dotnet core and C# to new developers.

## Coffee API Project
This is a simple web api project with single API that acceps coffee orders and pushes them to a Kafka topic.

**Nuget packages used**
- Confluent.Kafka

**Dockerise the API**

The next step is to dockerize the API.
To do that, follow these steps:

- Remove the KafkaConfiguration section from appsettings.json
- Add a docker file to the project for aspnet core. VS Code Docker extension can be used for the same. This [link](https://docs.docker.com/engine/examples/dotnetcore/) shows how this can be done manually.
- Run the docker build command.
- For docker run command, pass the Kafka configuration using the "ASPNETCORE_" prefix like this:
`docker run -p 8080:80 -e ASPNETCORE_KafkaConfiguration__BootStrapServers="{your Kafka bootstrap servers}" -e ASPNETCORE_KafkaConfiguration__SaslUsername="{Your Kafka Sasl username}" -e ASPNETCORE_KafkaConfiguration__SaslPasword="{Your Kafka Sasl Password}" -e ASPNETCORE_KafkaConfiguration__Topic="{Your Kafka Topic}"
--rm -it coffeeapi:latest`

## Coffee Worker App
This is a dotnet core worker app that subscribes as a consumer to the Kafka topic and prints the messages that are published on the topic.

**Nuget packages used**
- Confluent.Kafka
