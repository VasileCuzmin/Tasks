FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
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
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Api/Tasks.Api.csproj", "src/Tasks.Api/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Domain.Data/Tasks.Domain.Data.csproj", "src/Tasks.Domain.Data/"]
COPY --chown=$imageUser:$imageUserGroup ["src/domain/Tasks.RuntimeDomain/Tasks.Runtime.Domain.csproj", "src/domain/Tasks.RuntimeDomain/"]
COPY --chown=$imageUser:$imageUserGroup ["src/domain/Tasks.DefinitionDomain/Tasks.Definition.Domain.csproj", "src/domain/Tasks.DefinitionDomain/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Runtime.Application/Tasks.Runtime.Application.csproj", "src/Tasks.Runtime.Application/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Definition.Application/Tasks.Definition.Application.csproj", "src/Tasks.Definition.Application/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.PublishedLanguage/Tasks.PublishedLanguage.csproj", "src/Tasks.PublishedLanguage/"]
COPY --chown=$imageUser:$imageUserGroup ["src/MessageSerDes/MessageSerDes.csproj", "src/MessageSerDes/"]
RUN dotnet restore "src/Tasks.Api/Tasks.Api.csproj" --configfile=NuGet.config
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Api/", "src/Tasks.Api/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Domain.Data/", "src/Tasks.Domain.Data/"]
COPY --chown=$imageUser:$imageUserGroup ["src/domain/Tasks.RuntimeDomain/", "src/domain/Tasks.RuntimeDomain/"]
COPY --chown=$imageUser:$imageUserGroup ["src/domain/Tasks.DefinitionDomain/", "src/domain/Tasks.DefinitionDomain/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Runtime.Application/", "src/Tasks.Runtime.Application/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.Definition.Application/", "src/Tasks.Definition.Application/"]
COPY --chown=$imageUser:$imageUserGroup ["src/Tasks.PublishedLanguage/", "src/Tasks.PublishedLanguage/"]
COPY --chown=$imageUser:$imageUserGroup ["src/MessageSerDes/", "src/MessageSerDes/"]
WORKDIR "/src/src/Tasks.Api"
RUN dotnet build "Tasks.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Tasks.Api.csproj" -c Release -o /app

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

ARG APP_VERSION=0.0.0.0
ENV APP_VERSION=${APP_VERSION}

ARG APP_DATE=1900-01-01
ENV APP_DATE=${APP_DATE}

ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["/bin/bash", "-c", "test -f /vault/secrets/credentials.vault && echo 'INFO: Vault credentials loaded.' && \
            source /vault/secrets/credentials.vault || echo 'INFO: Vault file not loaded.' && dotnet Tasks.Api.dll"]