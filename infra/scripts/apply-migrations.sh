#!/bin/bash

DB_HOST=${DB_HOST:-postgres}
DB_PORT=${DB_PORT:-5432}
DB_USER=${DB_USER:-postgres}

echo "Waiting for database connection..."
while ! pg_isready -h $DB_HOST -p $DB_PORT -U $DB_USER -q; do
    echo "Database not ready yet, retrying in 2 seconds..."
    sleep 2
done

echo "Database connection established. Applying migrations..."
cd /src/src/SnackFlow.Infrastructure
dotnet ef database update
echo "Database migrations completed successfully."