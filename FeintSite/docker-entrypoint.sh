#!/bin/bash
until pg_isready -h db -p 5432;
          do echo "waiting for database"; sleep 2; done;
dotnet-ef database update
dotnet watch run