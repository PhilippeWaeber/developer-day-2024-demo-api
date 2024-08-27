# Demo.API
This repository contains the source code for the Demo.Api, presented during the 2024 Galenica Developer days. It demonstrates how Docker containers and test containers can facilitate development.

This topics are covered:
- **Creating a Simple REST API**: Build a basic API as the foundation of the demo.
- **Structured Logging with Serilog**: Integrate Serilog to generate structured logs, directing them to a local SEQ instance.
- **Querying Logs in SEQ**: Explore how to query and analyze structured logs using SEQ.
- **Database Integration**: Implement a SQL database using Entity Framework with a code-first approach.
- **Sandboxed Testing with SQL TestContainers**: Run component tests against a database hosted in a sandboxed SQL TestContainer.

Commits are structured to reflect the progression of the demo.

## Prerequisits
- [.NET core 8.0](https://dotnet.microsoft.com/download/dotnet/8.0): The .NET SDK is required to build and run the project.
- [Docker desktop](https://www.docker.com/products/docker-desktop): Docker is required to run SEQ and the SQL TestContainer.

### Set up SEQ
[Seq](https://datalust.co/seq) is a centralized logging server that provides a web UI for easy access to structured logs.

To set up Seq, follow these steps:

1. Ensure Docker Desktop is running.

1. Pull the Seq Docker image:
    ```ps
    docker pull datalust/seq
    ```

1. Run the Seq Docker container:
    ```ps
    docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
    ```
    This Docker command is running the datalust/seq image as a detached container named seq, with port 80 in the container mapped to port 5341 on the host, and an environment variable accepting the End-User License Agreement.
   Seq will now be accessible at [http://localhost:5341](http://localhost:5341).

For more detailed information, refer to the [Seq documentation](https://docs.datalust.co/docs/getting-started-with-docker).