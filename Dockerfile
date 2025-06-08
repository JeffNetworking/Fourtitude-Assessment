# Use SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

# Copy solution and projects
COPY *.sln ./
COPY Application/*.csproj ./Application/
COPY Fourtitude-Assessment/*.csproj ./Fourtitude-Assessment/

# Restore nuget packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish your main project
WORKDIR /app/Fourtitude-Assessment
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Set environment variable to listen on port 80
ENV ASPNETCORE_ENVIRONMENT=Development

# Copy published files from build stage
COPY --from=build /app/Fourtitude-Assessment/out ./

# Expose port 80
EXPOSE 80

ENTRYPOINT ["dotnet", "Fourtitude-Assessment.dll"]
