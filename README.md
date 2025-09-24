# Exploring OneAgent and OpenTelemetry monitoring with Azure Web App Services
In this tutorial we will be exploring how Dynatrace is able to monitor and ingest traces from an Azure Web App Service through the three main deployment options.

	• Native Dynatrace Azure Web App Service OneAgent Extension
	• OpenTelemetry (Otel) via OTLP
	• Native Dynatrace Azure Web App Service OneAgent Extension + OpenTelemetry 


Note:

* As the base application is a simple .NET application, this could be deployed in any other fashion (VM in the cloud, containerized, OneAgent on your Windows laptop, etc...).

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
	

## Pre-requisites: 

* Dynatrace SaaS tenant.

* Dynatrace API token for Otel ingest.

* Azure environment access.

## Steps:

* Clone the repo and open your IDE

* Run the 

## Outcome:

Native Dynatrace Azure Web App Service OneAgent Extension:

![alt text](https://github.com/dynatrace-bobbyvogler/dt-azure-web-app-service-otel/blob/main/images/oneagent.png?raw=true "OneAgent Extension")

OpenTelemetry (Otel) via OTLP:
![alt text](https://github.com/dynatrace-bobbyvogler/dt-azure-web-app-service-otel/blob/main/images/otel.png?raw=true "Otel")

Native Dynatrace Azure Web App Service OneAgent Extension + OpenTelemetry: 
![alt text](https://github.com/dynatrace-bobbyvogler/dt-azure-web-app-service-otel/blob/main/images/oneagent-otel.png?raw=true "OneAgent Extension + Otel")


### Author Notes:

As there is an Azure Web App Service free trial tier, I was able to deploy two of these branches to one App Service Plan, then had to create another App Service Plan for the remaining deployment.
![Azure App Service Limits](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits#azure-app-service-limits)

### For Dynatrace Sprint Tenants:

Downloading the OneAgent extension on azure requires a different server URL for the teant. 
By running the API call below we are able to grab an ActiveGate service that is assoicated with your sprint tenant.

```
/api/v1/deployment/installer/agent/connectioninfo/endpoints
```

For example, mine was:
```
https://xx-xx-xxxxxxx-xx-cluster-acceptancee2e.sprint.dynatracelabs.com/e/<tenantId>/api
```
