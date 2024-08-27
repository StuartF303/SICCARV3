# /*
# * Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
# * All rights reserved.
# *
# * This file is part of a proprietary software product developed by Siccar.
# *
# * This source code is licensed under the Siccar Proprietary Limited Use License.
# * Use, modification, and distribution of this software is subject to the terms
# * and conditions of the license agreement. The full text of the license can be
# * found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
# *
# * Unauthorized use, copying, modification, merger, publication, distribution,
# * sublicensing, and/or sale of this software or any part thereof is strictly
# * prohibited except as explicitly allowed by the license agreement.
# */

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
