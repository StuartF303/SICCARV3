AdminUI
updated: v1.1 Databox release

Dockerised:

Setyour environment

powershell

$env:FEED_ACCESSTOKEN = "your acr pat"
$env:SICCARSERVICE = "https://n0.siccar.dev"  # defaults to https://localhost:8443
$env:APPPATH = "/admin/" # defaults to / and must end in a '/' !!!

from solution root SICCARV3

	docker build -t adminui --build-arg FEED_ACCESSTOKEN --progress auto -f src\ui\AdminUI\Client\Dockerfile .	


	docker run -d -p 8888:80 --name adminui --env APPPATH --env SICCARSERVICE adminui:latest