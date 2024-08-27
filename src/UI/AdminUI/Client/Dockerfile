# simple dockerize Blazor Web Assembly Project to NGINX Host
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
ARG BUILDARGS=Debug
ENV BUILDCONFIG=$BUILDARGS
RUN curl -fsSL https://aka.ms/install-artifacts-credprovider.sh  | sh
COPY ["src/UI/AdminUI/Client/AdminUI.Client.csproj", "src/UI/AdminUI/Client/"]
COPY ["src/UI/AdminUI/Client/nuget.config", "src/UI/AdminUI/Client/"]
COPY ["src/SDK/SiccarApplicationClient/SiccarApplicationClient.csproj", "src/SDK/SiccarApplicationClient/"]
COPY ["src/SDK/SiccarApplicationClient/nuget.config", "src/SDK/SiccarApplicationClient/"]
#COPY ["AdminUI.Client.csproj", "."]
ARG FEED_ACCESSTOKEN
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://projectbob.pkgs.visualstudio.com/SICCARV3/_packaging/siccarv3feed/nuget/v3/index.json\", \"username\":\"docker\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"
RUN echo $VSS_NUGET_EXTERNAL_FEED_ENDPOINTS
RUN dotnet restore "src/UI/AdminUI/Client/AdminUI.Client.csproj"
RUN dotnet restore "src/SDK/SiccarApplicationClient/SiccarApplicationClient.csproj"
COPY . .

FROM base AS testrunner
ENV SICCARSERVICE=https://localhost:8443
ENV APPPATH="/"
WORKDIR /app/src/UI/AdminUI
RUN dotnet build "Client/AdminUI.Client.csproj" -c Release -o /app/build
COPY . .

FROM base AS publish
ENV SICCARSERVICE=https://localhost:8443
ENV APPPATH="/"
RUN echo $VSS_NUGET_EXTERNAL_FEED_ENDPOINTS
WORKDIR /app/src/UI/AdminUI
RUN dotnet publish "Client/AdminUI.Client.csproj" -p:BlazorEnableCompression=true -c Release -o /app/publish
COPY . .

FROM nginx:1.23.2-alpine AS final
WORKDIR /usr/share/nginx/html
ENV BLAZOR_ENVIRONMENT=Production
ENV APPPATH="/"
ENV SICCARSERVICE=https://localhost:8443/
EXPOSE 80
COPY --from=publish /app/publish/wwwroot .
COPY --from=publish /app/publish/wwwroot ./admin
COPY --from=publish /app/publish/nginx.conf /etc/nginx/nginx.conf
COPY --from=publish /app/publish/docker_start.sh /app/docker_start.sh
RUN sed -i 's/\r//g' /app/docker_start.sh

CMD /bin/sh /app/docker_start.sh