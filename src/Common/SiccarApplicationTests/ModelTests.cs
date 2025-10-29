// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

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
