@REM /*
@REM * Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
@REM * All rights reserved.
@REM *
@REM * This file is part of a proprietary software product developed by Siccar.
@REM *
@REM * This source code is licensed under the Siccar Proprietary Limited Use License.
@REM * Use, modification, and distribution of this software is subject to the terms
@REM * and conditions of the license agreement. The full text of the license can be
@REM * found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
@REM *
@REM * Unauthorized use, copying, modification, merger, publication, distribution,
@REM * sublicensing, and/or sale of this software or any part thereof is strictly
@REM * prohibited except as explicitly allowed by the license agreement.
@REM */

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