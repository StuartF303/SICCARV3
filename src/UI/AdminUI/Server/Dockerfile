# To run this Dockerfile you need:
# 1. An Azure DevOps PAT as a build arg FEED_ACCESSTOKEN (private Azure Feed requires a PAT)
# 2. Place the PAT value in a .env file and configure docker-compose-override.yml to use the build arg FEED_ACCESSTOKEN

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
ARG BUILDARGS=Release
ENV BUILDCONFIG=$BUILDARGS
# Install the Azure Artifact Credential Provider (will use a PAT)
RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh  | sh
COPY ["src/UI/AdminUI/Server/nuget.config", "src/UI/AdminUI/Server/"]
COPY ["src/UI/AdminUI/Server/AdminUI.Server.csproj", "src/UI/AdminUI/Server/"]
COPY ["src/UI/AdminUI/Client/nuget.config", "src/UI/AdminUI/Client/"]
COPY ["src/UI/AdminUI/Client/AdminUI.Client.csproj", "src/UI/AdminUI/Client/"]
COPY ["src/Common/SiccarCommon/SiccarCommon.csproj", "src/Common/SiccarCommon/"]
ARG FEED_ACCESSTOKEN
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://projectbob.pkgs.visualstudio.com/SICCARV3/_packaging/siccarv3feed/nuget/v3/index.json\", \"username\":\"docker\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"
RUN echo $VSS_NUGET_EXTERNAL_FEED_ENDPOINTS
RUN dotnet restore "src/UI/AdminUI/Server/AdminUI.Server.csproj"
COPY . .
RUN dotnet build "src/UI/AdminUI/Server/AdminUI.Server.csproj" -c ${BUILDCONFIG} -o /app/build

FROM build AS publish
RUN dotnet publish "src/UI/AdminUI/Server/AdminUI.Server.csproj" -c ${BUILDCONFIG} -o /app/publish

FROM build AS testrunner
WORKDIR /app/src/UI/AdminUI/Server
COPY src/Services/Tenant/TenantTests/*.csproj .
RUN dotnet restore TenantUnitTests.csproj
COPY src/Services/Tenant/TenantTests/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# copy the localhost test cert
COPY ["components/selfsigned.crt_","/usr/local/share/ca-certificates/localhost.crt"]
COPY ["components/selfsigned.crt_","/etc/ssl/certs/localhost.pem"]
RUN update-ca-certificates --fresh
ENTRYPOINT ["dotnet", "AdminServer.dll"]
