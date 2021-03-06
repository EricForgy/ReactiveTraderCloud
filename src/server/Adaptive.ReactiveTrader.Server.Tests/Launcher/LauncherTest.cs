﻿using System;
using System.IO;
using Adaptive.ReactiveTrader.Server.Launcher;
using FakeItEasy;
using Xunit;

namespace Adaptive.ReactiveTrader.Server.Tests.Launcher
{
    public class LauncherTest : IDisposable
    {
        private readonly IServiceLauncher _testServiceLauncher;
        private readonly Program _program;
        private readonly TextWriter _consoleOutput;
        private readonly StringWriter _writer;

        public LauncherTest()
        {
            _consoleOutput = Console.Out;
            _writer = new StringWriter();
            Console.SetOut(_writer);

            _testServiceLauncher = A.Fake<IServiceLauncher>();
            _program = new Program(_testServiceLauncher);
            _program.Stop();
        }

        [Fact]
        public void pnArgs_PricingServiceLaunchedAndInConsoleOutput()
        {
            var call = A.CallTo(() => _testServiceLauncher.StartService(ServiceType.Pricing));
            call.Returns("p1");

            _program.Run(new LauncherConfig() {ServicesToStart = new[] {ServiceType.Pricing}});


            call.MustHaveHappened(Repeated.Exactly.Once);

            var output = _writer.ToString();
            Assert.Contains("Started Pricing Service: p1", output);
        }

        [Fact]
        public void aInArgs_AnalyticsLaunchedAndInConsoleOutput()
        {
            var call = A.CallTo(() => _testServiceLauncher.StartService(ServiceType.Analytics));
            call.Returns("a1");

            _program.Run(new LauncherConfig() {ServicesToStart = new[] {ServiceType.Analytics}});

            call.MustHaveHappened(Repeated.Exactly.Once);

            var output = _writer.ToString();
            Assert.Contains("Started Analytics Service: a1", output);
        }

        [Fact]
        public void aAndInteractiveInArgs_AnalyticsLaunchedAndInteractiveInConsoleOutput()
        {
            var call = A.CallTo(() => _testServiceLauncher.StartService(ServiceType.Analytics));
            call.Returns("a1");

            _program.Run(new LauncherConfig() {ServicesToStart = new[] {ServiceType.Analytics}, IsInteractive = true});

            var output = _writer.ToString();

            call.MustHaveHappened(Repeated.Exactly.Once);
            Assert.Contains("Started Analytics Service: a1", output);
            Assert.Contains("INTERACTIVE MODE", output);
        }

        public void Dispose()
        {
            Console.SetOut(_consoleOutput);
        }
    }
}