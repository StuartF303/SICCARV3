@echo off
IF "%~1"=="" GOTO query
:process
docker-compose stop %1
docker-compose rm -f
docker-compose build --build-arg BUILDARGS=Development %1
docker-compose up -d %1
docker-compose ps
GOTO end
:query
echo Rebuilding all..
set /p proceed=Proceed (y)
IF "%proceed%"=="y" GOTO process
:end
time /t