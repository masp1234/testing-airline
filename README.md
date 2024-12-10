## Database setup
Have Docker installed and make sure is it running.

Run the database setup script by running

``` bash /backend/Database/Scripts/create-database.sh```

This will create the database and populate it with some test data.

## Environment variables
Put the environment variables below into a ```.env``` file

```
MYSQL_CONNECTION_STRING = "Server=localhost;Database=airline_project;User=root;Password=123123"
JWTSecretKey = "JWTSecret1234567sdasdsadadasdsadasdasdasdasdasdzxdvvbb"
Issuer = "issuer"
Audience = "audience"
CLIENT_URL = "http://localhost:5173"
```

## Start the application
Go into the ```/backend/``` folder and run ```dotnet run```.

Alternatively run ```dotnet run --project backend``` from the root folder.
