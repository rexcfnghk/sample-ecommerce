#!/bin/bash

# Wait 60 seconds for SQL Server to start up by ensuring that 
# calling SQLCMD does not return an error code, which will ensure that sqlcmd is accessible
# and that system and user databases return "0" which means all databases are in an "online" state
# https://docs.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-databases-transact-sql?view=sql-server-2022

DBSTATUS="1"
i=0
MSSQL_SA_PASSWORD=$(cat /run/secrets/sql_sa_password)

while [[ "$DBSTATUS" != "0" ]] && [[ "$i" -lt 60 ]]; do
	i=$(($i + 1))
	DBSTATUS="$(/opt/mssql-tools18/bin/sqlcmd -h -1 -t 1 -S host.docker.internal,1433 -U sa -P "$MSSQL_SA_PASSWORD" -Q "SET NOCOUNT ON; Select SUM(state) from sys.databases;" -C | tr -d ' ')"
	sleep 1
done

ERRCODE="$?"
if [[ "$DBSTATUS" != "0" ]] || [[ "$ERRCODE" -ne 0 ]]; then 
	echo "SQL Server took more than 60 seconds to start up or one or more databases are not in an ONLINE state"
	exit 1
fi

# Run the setup script to create the DB and the schema in the DB
/opt/mssql-tools18/bin/sqlcmd -S host.docker.internal,1433 -U sa -P "$MSSQL_SA_PASSWORD" -i /assets/setup.sql -C