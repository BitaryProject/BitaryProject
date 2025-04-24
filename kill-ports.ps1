# kill-ports.ps1
# This script kills processes using ports 5000 and 5001

Write-Host "Checking for processes using ports 5000 and 5001..."

# Find and kill process on port 5000
$port5000 = netstat -ano | findstr :5000 | findstr LISTENING
if ($port5000) {
    $pid5000 = ($port5000 -split '\s+')[-1]
    Write-Host "Killing process with PID $pid5000 on port 5000"
    taskkill /F /PID $pid5000
} else {
    Write-Host "No process found using port 5000"
}

# Find and kill process on port 5001
$port5001 = netstat -ano | findstr :5001 | findstr LISTENING
if ($port5001) {
    $pid5001 = ($port5001 -split '\s+')[-1]
    Write-Host "Killing process with PID $pid5001 on port 5001"
    taskkill /F /PID $pid5001
} else {
    Write-Host "No process found using port 5001"
}

Write-Host "Ports have been cleared. You can now start your application." 