# Project Chronos

Client: David Gutman

Lead & Infra: Aaron Iziyaev

Team: Noam Argaman, Adam Rammal, Shalev Kayat and Aaron Iziyaev, or in short - Team NASA.

## Local Development Guide

1. Start PostgreSQL Database

    ```bash
    docker-compose up postgres -d
    ```

2. Run the migrations to create the database schema:

    ```bash
    dotnet ef database update --project src/Chronos.Data --startup-project src/Chronos.MainApi
    ```

3. Start the API service

    ```bash
    docker-compose up --build
    ```
