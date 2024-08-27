# tag a named 
[CmdletBinding()]
param (
    [Parameter(Mandatory=$false)] [string] $releaseName='latest',
    [Parameter(Mandatory=$false)] [string] $releaseTag='release'
)

az account set --subscription "2122partnersub"
az acr import --name siccardev --source siccardev.azurecr.io/adminui:$releaseName --image adminui:$releaseTag --force
az acr import --name siccardev --source siccardev.azurecr.io/action-service:$releaseName --image action-service:$releaseTag --force
az acr import --name siccardev --source siccardev.azurecr.io/blueprint-service:$releaseName --image blueprint-service:$releaseTag --force
az acr import --name siccardev --source siccardev.azurecr.io/peer-service:$releaseName --image peer-service:$releaseTag --force
az acr import --name siccardev --source siccardev.azurecr.io/register-service:$releaseName --image register-service:$releaseTag --force
az acr import --name siccardev --source siccardev.azurecr.io/tenant-service:$releaseName --image tenant-service:$releaseTag --force
az acr import --name siccardev --source siccardev.azurecr.io/validator-service:$releaseName --image validator-service:$releaseTag --force
az acr import --name siccardev --source siccardev.azurecr.io/wallet-service:$releaseName --image wallet-service:$releaseTag --force
