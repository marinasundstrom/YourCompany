# Getting started

* Portal: https://acme.yourbrand.local:5174/ (Actually the reverse proxy)
* Identity Management: https://localhost:5040/
* Store: https://localhost:7188/

## Run dependencies in Docker

To run dependencies, like database, in Docker:

```
docker compose -f docker-compose.deps.yml up -d
```

## Seeding databases

You need to seed the database for each service.

```
dotnet run -- --seed
```

If you use VS Code, and you have the Restore Terminals extension installed, there will be a terminal for each service. Each with a command already specified.

## Running services

Just run each service:

```
dotnet run
```

Again, if you use VS Code, and you have the Restore Terminals extension installed, there will be a terminal for each service. Just add the ``--seed`` to the command.

### Important service

These services are essential for the function of YourBrand:

* Proxy
* Portal
* AppService
* IdentityManagement
* HumanResources
* ApiKeys

### Syncing user data

Everytime a database is created and recreated, you must populate it with users.

Initial creation from seed:

```
dotnet run --project Seeder/Seeder.csproj -- --seed
```

To sync users to services (will make sure users have been created):

```
dotnet run --project Seeder/Seeder.csproj -- --sync
```

The services must be running.

A ``CreateUser`` message will be published, and each service will consume that message, creating a local user if not already existing.

## Setting up a company

You have to create a company in the Portal, in ``Administration > Set up``.

Default credentials:

```
alice / bob
Pass123$
```

These credentials are used when logging in for the first time.

## DevTunnel

To connect from remote:

```
devtunnel host -p 5174 -a
```