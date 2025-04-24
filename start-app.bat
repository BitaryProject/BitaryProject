@echo off
echo Clearing ports before starting the application...
powershell -ExecutionPolicy Bypass -File kill-ports.ps1

echo Starting the application...
cd BitaryProject
dotnet run

echo Application startup complete. 