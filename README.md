# OTel

OpenTelemetry Code Samples

This project demonstrates how to set up and use OpenTelemetry (OTel) with various services for distributed tracing and observability. It includes:

- A **console app** (`SampleStack.Telemetry`) that acts as a client, sending requests to the API.
- A **web API** (`SampleStack.Telemetry.Api`) that processes requests and generates telemetry data.
- Supporting infrastructure for telemetry collection, processing, and visualization.

## Getting Started

### Prerequisites

- Docker and Docker Compose installed on your system.
- Make sure the required ports are available on your machine.

### Commands

To build the project and start all services, run the following commands:

```bash
make build && make start
```

### Accessing Services

Once the services are running, you can access the following dashboards:

- OpenSearch Dashboards: [http://127.0.0.1:5601/](http://127.0.0.1:5601/)
- Jaeger UI: [http://127.0.0.1:16686/](http://127.0.0.1:16686/)

### Project Structure

- `SampleStack.Telemetry.Api`: A web API that processes requests and generates telemetry data.
- `SampleStack.Telemetry`: A console app that acts as a client, sending requests to the API.
- `SampleStack.Telemetry.Generics`: A shared library containing reusable components and configurations.

### Telemetry Flow

1. The console app sends HTTP requests to the API.
2. Both the client and server generate telemetry spans using OpenTelemetry.
3. The spans are sent to the OpenTelemetry Collector, which forwards them to Data Prepper and Jaeger.
4. Data Prepper processes the spans and stores them in OpenSearch.
5. OpenSearch Dashboards and Jaeger visualize the telemetry data.
