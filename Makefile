# Build once, use everywhere
build:
		docker compose --env-file ./SampleStack.Telemetry/env_file.env -f ./SampleStack.Telemetry/docker-compose.yaml build

# Start all services
up:
		docker compose --env-file ./SampleStack.Telemetry/env_file.env -f ./SampleStack.Telemetry/docker-compose.yaml up -d

start:
		docker compose --env-file ./SampleStack.Telemetry/env_file.env -f ./SampleStack.Telemetry/docker-compose.yaml up

# Stop everything
down:
		docker compose --env-file ./SampleStack.Telemetry/env_file.env -f ./SampleStack.Telemetry/docker-compose.yaml down

# Rebuild and restart (when dependencies change)
rebuild: build down up