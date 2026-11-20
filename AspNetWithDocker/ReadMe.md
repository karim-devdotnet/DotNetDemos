# Containerize an ASP.NET Core Web API with Docker
By [Mwiza Kumwenda](https://mwiza.medium.com/containerize-an-asp-net-core-web-api-with-docker-707fe8d8e45d)

**Containerize a Web API for easy mocking and testing by Front-End developers.

---

As Front-End developers, we are sometimes faced with situations where we need to run back-end code on our local Dev machines just to make API requests. This is when Docker can come in handy.

Today we will look at how we can containerize an ASP.NET Web API using Docker. By so doing, instead of running back-end code on your Dev machine, you instead just spin up an instance of a Docker image. The good thing is that you can run the same Docker image in different environments without other extra configurations and customization.

## Prerequisites & Tools
* ASP.NET Core SDK 11
* Docker
* Visual Studio Code (VS Code) with the Docker extension by Microsoft installed (press Ctrl + Shift + X, search for "Docker", and install).

---

### Step 1: Create the ASP.NET Core Web API

Create a new Web API project named IntroToMvc by running the following command within your projects folder:

```Bash
dotnet new webapi -o IntroToMvc --no-http
```

Navigate to the project's root directory and run the application:

```Bash
dotnet run
```

Open a web browser and go to http://localhost:5000/WeatherForecast to verify you receive the sample JSON output.

### Step 2: Generate Docker Files
Open the VS Code command palette by pressing Ctrl + Shift + P.
Type Docker: Add and select Docker: Add Docker Files to Workspace...
Select ASP.NET Core as the application platform.
Select your target operating system (e.g., Linux).
VS Code will generate a .dockerignore file (specifying which files to exclude from the image) and a Dockerfile in your project's root folder with contents similar to this:

```bash
Dockerfile
FROM mcr.microsoft.com/dotnet/core/aspnet:11 AS base
WORKDIR /app
EXPOSE 5207

ENV ASPNETCORE_URLS=http://+:5207

FROM mcr.microsoft.com/dotnet/core/sdk:11 AS build
WORKDIR /src
COPY ["IntroToMvc.csproj", "./"]
RUN dotnet restore "./IntroToMvc.csproj"
COPY . .
# WORKDIR "/src/."
RUN dotnet build "IntroToMvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IntroToMvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntroToMvc.dll"]
```

### Step 3: Build the Docker Image
Run the following command in the folder where the Dockerfile is located to build an image named dockerwebapi:

```Bash
docker buildx build -t dockerwebapi -f Dockerfile .
```
To verify that the image was created, run:

```Bash
docker images
```
In addition to dockerwebapi, you will see the base images for the ASP.NET Core runtime and SDK specified in the Dockerfile.

### Step 4: Run the Docker Container
As specified in the Dockerfile, the container exposes port 5207. Map it to local port 8080 and start the container:

```Bash
docker run -ti --rm --name myfirstwebapiwithdocker -p 8080:5207 dockerwebapi
```
Navigate to http://localhost:8080/WeatherForecast in your browser to view the JSON data served directly from your running Docker container.

## Distribution
You can now make API requests against this container without running back-end source code locally. To share this API with other developers, simply provide a copy of the Docker image or push it to a container registry like Docker Hub or Azure Container Registry.

## Push image to Docker

### 1. In PowerShell, login to docker hub with:
```bash
docker login
 ```
### 2. Tag your image
Replace YOUR_USERNAME with your Docker Hub username:
```bash
docker tag dockerwebapi:latest YOUR_USERNAME/dockerwebapi:latest
 ```

 ### 3. Push the image
 ```bash
docker push YOUR_USERNAME/dockerwebapi:latest
 ```

 ### 4. Verifiy:
 Visit: [Docker Hub repositories](https://hub.docker.com/repositories)
Docker Hub repositoriesYou should see dockerwebapi in your repositories.

### 5. Complete command sequence
```bash
docker login
docker tag dockerwebapi:latest YOUR_USERNAME/dockerwebapi:latest
docker push YOUR_USERNAME/dockerwebapi:latest
 ```