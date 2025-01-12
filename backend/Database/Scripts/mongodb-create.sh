#!/bin/bash

container_name=airline_mongodb
user=root
password=123123

# Stop og fjern eksisterende container, hvis den findes
docker stop $container_name && docker rm $container_name

# Kør MongoDB containeren med de nødvendige miljøvariabler og scripts
docker run -d \
  --name $container_name \
  -e MONGO_INITDB_ROOT_USERNAME=$user \
  -e MONGO_INITDB_ROOT_PASSWORD=$password \
  -v $(pwd)/../MongoScripts/init-mongo.js:/docker-entrypoint-initdb.d/init-mongo.js:ro \
  -p 27017:27017 \
  mongo 

# Tjek om containeren er startet korrekt
if [ $? -eq 0 ]; then
    echo "MongoDB container started successfully!"
else
    echo "Error starting MongoDB container."
    exit 1
fi

# Vent på, at containeren starter op, og at init-mongo.js bliver kørt
echo "Waiting for MongoDB container to initialize..."
sleep 10  # Juster ventetiden efter behov


# Bekræft at MongoDB er tilgængelig
docker exec -it $container_name mongosh -u $user -p $password --authenticationDatabase admin --eval "db.runCommand({ connectionStatus: 1 })"

if [ $? -eq 0 ]; then
    echo "MongoDB is up and running!"
else
    echo "Error connecting to MongoDB."
    exit 1
fi

echo "Done!"