# Set environment variable for Production
$env:ASPNETCORE_ENVIRONMENT = "Production"

# Define the path to your project
$projectPath = "./HackerNewsApi/"

# Run the project in Release mode with the correct profile
dotnet run --configuration Release --launch-profile "Production - https" --project $projectPath