#!/bin/sh
# make sure this file is not save with CRLF line endings!
echo Starting WASM Host
echo Setting SiccarServer : $SICCARSERVICE
#echo Setting AppPath : $APPPATH
# The script replaces some values in the wwwroot appsettings
# first of is the Siccar Service we will be installed / delivered from
/bin/sed -i -e 's|https://localhost:8443/|'"$SICCARSERVICE"'|g' /usr/share/nginx/html/admin/appsettings.json
# second is the path we with to run from "AppPath": "/"
#/bin/sed -i -e 's|\"/\"|'\""$APPPATH"\"'|g' /usr/share/nginx/html/appsettings.json
#lets just go for the HTML as well
#/bin/sed -i -e 's|base href=\"/\"|base href=\"'"$APPPATH"\"'|g' /usr/share/nginx/html/index.html
# Start nginx service in the container
nginx -g 'daemon off;'
