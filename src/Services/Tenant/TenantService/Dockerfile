# To run this Dockerfile you need:
# 1. An Azure DevOps PAT as a build arg FEED_ACCESSTOKEN (private Azure Feed requires a PAT)
# 2. Place the PAT value in a .env file and configure docker-compose-override.yml to use the build arg FEED_ACCESSTOKEN
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
ENV ASPNETCORE_URLS=http://+:8080;https://+:8443

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /app
ARG BUILDARGS=Release
ENV BUILDCONFIG=$BUILDARGS
#RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh  | sh
RUN curl -fsSL https://aka.ms/install-artifacts-credprovider.sh  | sh
COPY ["src/Services/Tenant/TenantService/TenantService.csproj", "src/Services/Tenant/TenantService/"]
COPY ["src/Services/Tenant/TenantCore/TenantCore.csproj", "src/Services/Tenant/TenantCore/"]
COPY ["src/Services/Tenant/TenantRepository/TenantRepository.csproj", "src/Services/Tenant/TenantRepository/"]
COPY ["src/Services/Tenant/TenantService/nuget.config", "src/Services/Tenant/TenantService/"]
COPY ["src/Services/Tenant/TenantTests/TenantUnitTests.csproj", "src/Services/Tenant/TenantTests/"]
COPY ["src/Services/Tenant/TenantTests/nuget.config", "src/Services/Tenant/TenantTests/"]
COPY ["src/Common/SiccarCommon/SiccarCommon.csproj", "src/Common/SiccarCommon/"]
COPY ["src/Common/SiccarCommonServiceClients/SiccarCommonServiceClients.csproj", "src/Common/SiccarCommonServiceClients/"]
ARG FEED_ACCESSTOKEN
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://projectbob.pkgs.visualstudio.com/SICCARV3/_packaging/siccarv3feed/nuget/v3/index.json\", \"username\":\"docker\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"
RUN echo $VSS_NUGET_EXTERNAL_FEED_ENDPOINTS
RUN dotnet restore "src/Services/Tenant/TenantService/TenantService.csproj"
COPY . .
# tenant cant handle thi s- conflict w/ automapper and idsrv4 RUN dotnet add "src/Services/Tenant/TenantService/TenantService.csproj" package Microsoft.ApplicationInsights.Kubernetes.HostingStartup
RUN dotnet build "src/Services/Tenant/TenantService/TenantService.csproj" -c ${BUILDCONFIG} -o /app/build

FROM build AS publish
RUN dotnet publish "src/Services/Tenant/TenantService/TenantService.csproj" -c ${BUILDCONFIG} -o /app/publish

FROM build AS testrunner
WORKDIR /app/src/Services/Tenant/tests
COPY src/Services/Tenant/TenantTests/*.csproj .
COPY src/Services/Tenant/TenantTests/nuget.config .
RUN dotnet restore TenantUnitTests.csproj
COPY src/Services/Tenant/TenantTests/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM base AS final
# Create a group and user so we are not running our container and application as root and thus user 0 which is a security issue.
RUN addgroup --system --gid 2600 siccargroup \
    && adduser --system --uid 1000 --ingroup siccargroup --shell /bin/sh siccaruser
WORKDIR /app
RUN mkdir -p /local/
RUN mkdir -p /app/data
RUN mkdir -p /app/data/keys
COPY --from=publish /app/publish .
RUN chown siccaruser:siccargroup /local
RUN chown siccaruser:siccargroup /app
RUN chown siccaruser:siccargroup /app/data
RUN chown siccaruser:siccargroup /app/data/keys
# copy the localhost test cert
COPY ["components/selfsigned.crt_","/usr/local/share/ca-certificates/localhost.crt"]
COPY ["components/selfsigned.crt_","/etc/ssl/certs/localhost.pem"]
COPY ["components/selfsigned.pfx_","default.pfx"]

RUN update-ca-certificates --fresh
#USER 1000
ENTRYPOINT ["dotnet", "TenantService.dll"]
