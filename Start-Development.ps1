# Set environment variable for Production
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Define the path to your project
$projectPath = "./HackerNewsApi/"

# Run the project in Debug mode with the correct profile
dotnet run --configuration Debug --launch-profile "Development - https" --project $projectPath