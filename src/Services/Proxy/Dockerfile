# Build a reverse proxy in nginx for the Siccar Microservices

FROM nginx  

COPY ["src/Services/Proxy/nginx.conf", "/etc/nginx/nginx.conf"]
COPY ["src/Services/Proxy/html/*", "/etc/nginx/html/"]
COPY ["src/Services/Proxy/localhost.crt", "/etc/ssl/certs/localhost.crt"]
COPY ["src/Services/Proxy/localhost.key", "/etc/ssl/private/localhost.key"]

