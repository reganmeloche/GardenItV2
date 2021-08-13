# GardenItV2

Backend project for IoT Gardening system. 

Exposes various endpoints to interact with Garden data. Also interacts with a MQTT broker (pub/sub) which connects with a NodeMCU device.

Data is stored in Postgres via Entity Framework. CloudMQTT is the MQTT broker.

To run: `dotnet run`

Deployed from Github Actions using Azure Container Registry
