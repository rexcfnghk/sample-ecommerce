#!/bin/bash

# Start the script to create the DB and user
/assets/configure-db.sh &

# Start SQL Server
/opt/mssql/bin/sqlservr
