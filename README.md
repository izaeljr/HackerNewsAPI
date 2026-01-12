# HackerNews API – Best Stories

This application is a **REST API built with ASP.NET Core** that returns the **Top N stories from Hacker News**, ordered by **score**, with the following characteristics:

* **Parallel processing** (`Parallel.ForEachAsync`)
* **In-memory caching**
* **Concurrency limiting** to protect the external API

---

## Prerequisites

* **.NET SDK 8.0 or later**
  [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

---

## Project Structure

```text
HackerNewsAPI/
│
├── Configuration/
│   └── HackerNewsOptions.cs
│
├── Models/
│   ├── HackerNewsItem.cs
│   └── StoryResponse.cs
│
├── Services/
│   ├── Interfaces/
│   │   └── IHackerNewsClient.cs
│   └── HackerNewsClient.cs
│
├── appsettings.json
├── Program.cs
└── HackerNewsAPI.csproj
```

---

## Configuration

### appsettings.json

The `appsettings.json` file should be located at the root of the project;

```json
{
  "HackerNews": {
    "BaseUrl": "https://hacker-news.firebaseio.com/v0",
    "MaxConcurrency": 10,
    "CacheMinutes": 5
  }
}
```

### Configuration Options

| Property         | Description                                |
| ---------------- | ------------------------------------------ |
| `BaseUrl`        | Base URL of the Hacker News API            |
| `MaxConcurrency` | Maximum number of concurrent HTTP requests |
| `CacheMinutes`   | Cache lifetime in minutes                  |

---

## Running the Application Locally

### Option 1 – Using the terminal

From the project root directory:

```bash
dotnet restore
dotnet run
```

The API will start on ports to:

```text
https://localhost:59405
http://localhost:59406
```

---

### Option 2 - Using Docker

This project includes a Dockerfile and can be run as a containerized application.

**Prerequisites**

Docker (native Linux, WSL, or Docker Desktop)

**Build the Docker image**

From the project root directory:

`docker build -t hackernews-api .`

**Run the container**

`docker run -d \
  --name hackernews-api \
  -p 8080:8080 \
  hackernews-api`

The API will be available at:

http://localhost:8080

Swagger UI:

http://localhost:8080/swagger

Stop and remove the container

`docker stop hackernews-api`
`docker rm hackernews-api`

---

### Option 3 – Using Docker Compose

Docker Compose simplifies running the application by defining the container configuration in a single file.

**Run with Docker Compose**

From the project root directory:

`docker compose up --build`

To run in detached mode:

`docker compose up -d --build`

Access the application
http://localhost:8080/swagger

Stop the application

`docker compose down`

---

### Option 4 – Using Visual Studio

1. Open the `HackerNewsAPI.sln` file
2. Select the **HTTPS** launch profile
3. Press **F5** or **Ctrl + F5**

---

## Swagger (API Testing UI)

After starting the application, open:

```
https://localhost:59405/swagger
```

## Available Endpoint

### GET /beststories

Returns the **Top N Hacker News stories**, ordered by score.

#### Example request:

```http
GET /beststories?n=5
```

#### Example response:

```json
[
  {
    "title": "The struggle of resizing windows on macOS Tahoe",
    "uri": "https://noheger.at/blog/2026/01/11/the-struggle-of-resizing-windows-on-macos-tahoe/",
    "postedBy": "happosai",
    "time": "2026-01-11T20:47:55+00:00",
    "score": 1113,
    "commentCount": 489
  }
]
```

---

### Parallelism

* Implemented using `Parallel.ForEachAsync`
* Concurrency is limited via the `MaxConcurrency` setting

### Caching

* In-memory caching using `IMemoryCache`
* Cache key is based on the requested value of `n`
* Prevents repeated calls to the external Hacker News API

---

## Important Notes

* The Hacker News API **does not require authentication**
* Excessive concurrency may lead to throttling or slow responses
* Caching is essential for performance and stability

---

## Possible Future Improvements

* Distributed cache (Redis)
* Background refresh using `IHostedService`
* Retry and circuit breaker policies (Polly)
* Rate limiting per client/IP
* Deploy in a cloud environment
* Unit tests

---

## Author

This project was developed as a **technical assessment using ASP.NET Core**, with a focus on:

* Best practices
* Scalability

---

If you have any questions or need help running the project, feel free to ask
