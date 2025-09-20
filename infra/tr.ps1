# Check if Terraform is already installed
if (Get-Command terraform -ErrorAction SilentlyContinue) {
    Write-Host "Terraform is already installed."
    exit 0
}

# Try winget first
Write-Host "Attempting to install Terraform with winget..."
winget install --id HashiCorp.Terraform -e --source winget
if ($LASTEXITCODE -eq 0) {
    Write-Host "Terraform installed successfully with winget."
    exit 0
}

Write-Host "Winget installation failed. Falling back to manual installation."

# Determine the latest version
$latestVersion = (Invoke-RestMethod -Uri "https://checkpoint-api.hashicorp.com/v1/check/terraform").current_version
Write-Host "Latest Terraform version: $latestVersion"

# Define URLs
$downloadUrl = "https://releases.hashicorp.com/terraform/${latestVersion}/terraform_${latestVersion}_windows_amd64.zip"
$zipFile = "$env:TEMP\terraform_${latestVersion}_windows_amd64.zip"
$installDir = "$env:USERPROFILE\AppData\Local\Terraform\bin"

# Create the installation directory if it doesn't exist
New-Item -ItemType Directory -Path $installDir -Force | Out-Null

# Download the zip file
Invoke-WebRequest -Uri $downloadUrl -OutFile $zipFile

# Extract the zip file
Expand-Archive -Path $zipFile -DestinationPath $installDir -Force

# Add installDir to the user PATH if not already present
$currentPath = [Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::User)
if ($currentPath -notlike "*$installDir*") {
    [Environment]::SetEnvironmentVariable("Path", $currentPath + ";$installDir", [EnvironmentVariableTarget]::User)
    $env:Path += ";$installDir"
}

# Verify installation
terraform version