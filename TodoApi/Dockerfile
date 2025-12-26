
#Stage 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

#Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

#Copy the rest of the files and build the project
COPY . ./
RUN dotnet publish -c Release -o out

#Stage 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

#Expose port
EXPOSE 8080

#Set environment variables for URLs
ENV ASPNETCORE_URLS=http://+:8080

#Run the app
ENTRYPOINT ["dotnet", "TodoApi.dll"]



