#!/bin/bash
# ============================================================================
# Docker entrypoint for SQL Server with auto-initialization
# ============================================================================
# Starts SQL Server in the background, waits for it to accept connections,
# then runs the initialization script to create the database and seed data.
# ============================================================================

set -e

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo "Waiting for SQL Server to start..."
for i in {1..60}; do
    if /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" -C -b &>/dev/null; then
        echo "SQL Server is ready."
        break
    fi
    echo "  Attempt $i/60 — not ready yet, waiting 2s..."
    sleep 2
done

# Run the initialization script
if [ -f /docker-entrypoint-initdb.d/docker-init.sql ]; then
    echo "Running docker-init.sql..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -i /docker-entrypoint-initdb.d/docker-init.sql -C -b
    echo "Database initialization complete."
else
    echo "No docker-init.sql found, skipping initialization."
fi

# Wait for SQL Server process
wait $SQL_PID
