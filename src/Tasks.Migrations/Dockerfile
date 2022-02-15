FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

ARG imageUser=tsuser
ARG imageUserGroup=tsgroup
ARG imageUserId=1375
ARG imageUserGroupId=1375

RUN addgroup --system --gid $imageUserGroupId $imageUserGroup && \     
    adduser --system --uid $imageUserId --ingroup $imageUserGroup $imageUser

RUN mkdir -p /app && \
    chown -R "$imageUser:$imageUserGroup" /app && \
    chmod -R 777 /app

USER $imageUser

COPY --chown=$imageUser:$imageUserGroup NuGet.config .
COPY --chown=$imageUser:$imageUserGroup dependencies.props .
COPY --chown=$imageUser:$imageUserGroup Directory.Build.props .
COPY --chown=$imageUser:$imageUserGroup src/_ExternalResources/Subtext.Scripting/ src/_ExternalResources/Subtext.Scripting/
COPY --chown=$imageUser:$imageUserGroup src/Tasks.Migrations/Tasks.Migrations.csproj src/Tasks.Migrations/
RUN dotnet restore src/Tasks.Migrations/Tasks.Migrations.csproj
COPY --chown=$imageUser:$imageUserGroup src/Tasks.Migrations/ src/Tasks.Migrations/
WORKDIR /src/src/Tasks.Migrations
RUN dotnet build Tasks.Migrations.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Tasks.Migrations.csproj -c Release -o /app

FROM base AS final
WORKDIR /app

ARG imageUser=tsuser
ARG imageUserGroup=tsgroup
ARG imageUserId=1375
ARG imageUserGroupId=1375

RUN addgroup --system --gid $imageUserGroupId $imageUserGroup && \     
    adduser --system --uid $imageUserId --ingroup $imageUserGroup $imageUser

USER $imageUser

COPY --chown=$imageUser:$imageUserGroup --from=publish /app .

ENTRYPOINT ["/bin/bash", "-c", "test -f /vault/secrets/credentials.vault && echo 'INFO: Vault credentials loaded.' && \
            source /vault/secrets/credentials.vault || echo 'INFO: Vault file not loaded.' && dotnet Tasks.Migrations.dll"]