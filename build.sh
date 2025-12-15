dotnet publish -c Release -r linux-arm64

# Build the Docker image
docker build -t niceygy/eddatacollector.net .

# Tag the Docker image
docker tag niceygy/eddatacollector.net ghcr.io/niceygy/eddatacollector.net:latest

# Push the Docker image to GH registry
docker push ghcr.io/niceygy/eddatacollector.net:latest

#Update local container

cd /opt/stacks/elite_db

docker compose pull

docker compose down

docker compose up -d

docker logs eddn_connector -f

#101357577
#101357577x