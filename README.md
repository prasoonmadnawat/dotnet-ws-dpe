# Dotnet Workshop
This repository contains a set of projects that will help introduce dotnet core and C# to new developers.

## Coffee API Project
This is a simple web api project with single API that acceps coffee orders and pushes them to a Kafka topic.

**Nuget packages used**
- Confluent.Kafka

## Coffee Worker App
This is a dotnet core worker app that subscribes as a consumer to the Kafka topic and prints the messages that are published on the topic.

**Nuget packages used**
- Confluent.Kafka
