# Exploring OneAgent and OpenTelemetry monitoring with Azure Web App Services
In this tutorial we will be exploring how Dynatrace is able to monitor and ingest traces from an Azure Web App Service through the three main deployment options.

	• Native Dynatrace Azure Web App Service OneAgent Extension
	• OpenTelemetry (Otel) via OTLP
	• Native Dynatrace Azure Web App Service OneAgent Extension + OpenTelemetry 

## This repo includes three branches for each of the above deployment options. 
### Main:

Basic .NET application to be deployed to an App Service and monitored with the Dynatrace OneAgent on Azure App Service.
------
### Otel:

Basic .NET application to be deployed or ran locally to colelct Otel traces and metadata and send via OTLP to the Dynatrace SaaS endpoint. 
------
### Otel-oneagent:

Basic .NET application to be deployed to an App Service, monitored with the Dynatrace OneAgent on Azure App Service and trace generation through Otel.
------
	

Pre-requisites: 

Dynatrace SaaS tenant
Dynatrace API token for Otel ingest
Azure environment access

Outcome:

Native Dynatrace Azure Web App Service OneAgent Extension:

Inline-style: 
![alt text](https://github.com/dynatrace-bobbyvogler/dt-azure-web-app-service-otel/blob/main/images/oneagent.png?raw=true "OneAgent Extension")

OpenTelemetry (Otel) via OTLP:
![alt text](https://github.com/dynatrace-bobbyvogler/dt-azure-web-app-service-otel/blob/main/images/otel.png?raw=true "Otel")

Native Dynatrace Azure Web App Service OneAgent Extension + OpenTelemetry: 
![alt text](https://github.com/dynatrace-bobbyvogler/dt-azure-web-app-service-otel/blob/main/images/oneagent-otel.png?raw=true "OneAgent Extension + Otel")
