using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Avalonia;
using LLEAV.Views.Windows;
using LLEAV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using Avalonia.ReactiveUI;
using System.Diagnostics;
using LLEAVTest;
using LLEAVTest.Windows;



[assembly: TestFramework("LLEAVTest.AvaloniaUiTestFramework", "LLEAVTest")]
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace LLEAVTest;


public class AvaloniaUiTestFramework : XunitTestFramework
{
    public AvaloniaUiTestFramework(IMessageSink messageSink)
        : base(messageSink)
    {

    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        => new Executor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);

    private class Executor : XunitTestFrameworkExecutor
    {
        public Executor(
            AssemblyName assemblyName,
            ISourceInformationProvider sourceInformationProvider,
            IMessageSink diagnosticMessageSink)
            : base(
                assemblyName,
                sourceInformationProvider,
                diagnosticMessageSink)
        {

        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
        {
            executionOptions.SetValue("xunit.execution.DisableParallelization", false);
            using var assemblyRunner = new Runner(
                TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink,
                executionOptions);

            await assemblyRunner.RunAsync();
        }
    }

    private class Runner : XunitTestAssemblyRunner
    {
        public Runner(
            ITestAssembly testAssembly,
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink diagnosticMessageSink,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
            : base(
                testAssembly,
                testCases,
                diagnosticMessageSink,
                executionMessageSink,
                executionOptions)
        {

        }

        public override void Dispose()
        {
            AvaloniaApp.Stop();

            base.Dispose();
        }

        protected override void SetupSyncContext(int maxParallelThreads)
        {
            var tcs = new TaskCompletionSource<SynchronizationContext>();
            var thread = new Thread(() =>
            {
                try
                {
                    AvaloniaApp
                        .BuildAvaloniaApp()
                         .AfterSetup(_ =>
                         {
                             tcs.SetResult(SynchronizationContext.Current);
                         })
                        .StartWithClassicDesktopLifetime(new string[0]);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            })
            {
                IsBackground = true
            };

            thread.Start();

            SynchronizationContext.SetSynchronizationContext(tcs.Task.Result);
        }
    }
}


public static class AvaloniaApp
{

    public static void Stop()
    {
        var app = GetApp();
        if (app is IDisposable disposable)
        {
            Dispatcher.UIThread.Post(disposable.Dispose);
        }
        Dispatcher.UIThread.Post(() => app.Shutdown());
    }


    public static IClassicDesktopStyleApplicationLifetime GetApp() =>
        (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

    public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .UseReactiveUI();
}
