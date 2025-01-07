#!/bin/bash

# Define variables
CONTAINER_NAME="neo4j_container"
NEO4J_USER="neo4j"       # Neo4j default username
NEO4J_PASSWORD="123123123"  # Password to set, must be min 8 characters.
NEO4J_VERSION="5.14.0"   # Specify Neo4j version (adjust as needed)
NEO4J_PORT="7474"        # HTTP port for Neo4j
NEO4J_BOLT_PORT="7687"   # Bolt protocol port
script_dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"  # Directory of this script
CYPHER_SCRIPT="${script_dir}/../Cypher/airline_system_2.cyp"  # Cypher script to execute

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "Docker is not installed. Please install Docker first."
    exit 1
fi

# Pull the Neo4j Docker image
echo "Pulling Neo4j Docker image (version $NEO4J_VERSION)..."
docker pull neo4j:$NEO4J_VERSION

# Stop and remove existing container (if any)
if docker ps -a --format '{{.Names}}' | grep -q "^$CONTAINER_NAME$"; then
    echo "Removing existing container with name $CONTAINER_NAME..."
    docker stop $CONTAINER_NAME
    docker rm $CONTAINER_NAME
fi

# Run the Neo4j Docker container
echo "Starting Neo4j container..."
docker run \
  --name $CONTAINER_NAME \
  -p $NEO4J_PORT:7474 \
  -p $NEO4J_BOLT_PORT:7687 \
  -e NEO4J_AUTH="$NEO4J_USER/$NEO4J_PASSWORD" \
  -d neo4j:$NEO4J_VERSION

if [ $? -eq 0 ]; then
    echo "Neo4j container started successfully!"

    # Wait for Neo4j to be fully started
    echo "Waiting for Neo4j to be ready..."
    sleep 20

    # Execute Cypher script if it exists
    if [ -f "$CYPHER_SCRIPT" ]; then
        echo "Executing Cypher script: $CYPHER_SCRIPT"
        docker exec -i $CONTAINER_NAME cypher-shell -u $NEO4J_USER -p $NEO4J_PASSWORD < "$CYPHER_SCRIPT"
        sleep 30
        if [ $? -eq 0 ]; then
            echo "Cypher script executed successfully!"
        else
            echo "Failed to execute Cypher script."
        fi
    else
        echo "Cypher script not found at: $CYPHER_SCRIPT"
    fi
else
    echo "Failed to start Neo4j container."
    exit 1
fi
