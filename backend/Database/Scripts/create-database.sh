#!/bin/bash

container_name=airline_mysql
script_dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
database_name=airline_project
user=root
password=123123

docker stop $container_name && docker rm $container_name

docker run --name $container_name -e MYSQL_ROOT_PASSWORD=$password -p 3306:3306 -d mysql
echo "Starting $container_name container..."
sleep 20
echo "Container started. Creating database..."
docker exec -i $container_name mysql -u${user} -p${password} -e "CREATE DATABASE ${database_name};"
docker exec -i $container_name mysql --user="${user}" --database="${database_name}" --password="${password}" < "${script_dir}/../SQL/airline_system.sql"
docker exec -i $container_name mysql --user="${user}" --database="${database_name}" --password="${password}" < "${script_dir}/../SQL/sample_data.sql"


if [ $? -eq 0 ]; then
    echo "Database created successfully!"
else
    echo "Error creating database."
    exit 1
fi

echo "Done!"