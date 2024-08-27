/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

using System;
using Xunit;
using Siccar.Application;

namespace SiccarApplicationTests
{
    public class ModelTests
    {
        [Fact]
        public void BluePrint_Test()
        {
            var t = new Blueprint();

            Assert.NotNull(t);

        }

        [Fact]
        public void Action_Test()
        {
            var t = new Siccar.Application.Action();

            Assert.NotNull(t);

        }
        [Fact]
        public void Control_Test()
        {
            var t = new Siccar.Application.Control();

            Assert.NotNull(t);

        }
        [Fact]
        public void Participant_Test()
        {
            var t = new Siccar.Application.Participant();

            Assert.NotNull(t);

        }
        [Fact]
        public void Disclosure_Test()
        {
            var t = new Siccar.Application.Disclosure();

            Assert.NotNull(t);

        }
        [Fact]
        public void Condition_Test()
        {
            var t = new Siccar.Application.Condition();

            Assert.NotNull(t);

        }
    }
}
