using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using LLEAV;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.GOM;
using LLEAV.Models.Algorithms.GOM.StateChange;
using LLEAV.Models.Algorithms.MIP;
using LLEAV.Models.Algorithms.MIP.StateChange;
using LLEAV.Models.Algorithms.ROM;
using LLEAV.Models.Algorithms.ROM.StateChange;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.GrowthFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.TerminationCriteria;
using LLEAVTest.Windows;
using ReactiveUI;
using System.Collections;
using Xunit.Abstractions;


namespace LLEAVTest;


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

public class TestClass
{
    protected IList<Action> tests;
    protected readonly ITestOutputHelper output;

    public TestClass(ITestOutputHelper testOutputHelper)
    {
        output = testOutputHelper;
    }

    public void Execute()
    {
        foreach (Action a in tests)
        {
            a.Invoke();
        }
    }
}

public class TestUI
{
    private readonly ITestOutputHelper _out;
    private TestClass[] testClasses;

    public static List<Exception> Exceptions = new List<Exception>();

    public TestUI(ITestOutputHelper testOutputHelper)
    {
        _out = testOutputHelper;

        testClasses = [
                new MainWindowTest(_out),
            new PopulationWindowTest(_out),
            new IterationDetailsWindowTest(_out),
        ];
    }
    [Fact]
    public void Test()
    {
        AvaloniaApp
             .BuildAvaloniaApp()
              .AfterSetup(_ =>
              {
                  TestExecution();
              })
             .StartWithClassicDesktopLifetime(new string[0]);
        if (Exceptions.Count > 0)
        {
            throw Exceptions[0];
        }
    }

    private void TestExecution()
    {
        Thread t = new Thread(new ThreadStart(() =>
        {
            Thread.Sleep(1000);
            foreach (TestClass t in testClasses)
            {
                t.Execute();
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
            AvaloniaApp.Stop();
        }));
        t.Start();

    }
}

public class Expect
{
    public static void Fail(string message)
    {
        try
        {
            Assert.Fail(message);
        }
        catch (Exception e)
        {
            TestUI.Exceptions.Add(e);
        }
    }

    public static void Equal(object expected, object actual, string message)
    {
        if (!expected.Equals(actual))
        {
            Fail(message);
        }
    }

    public static void True(bool value, string message = "Expected True!")
    {
        if (!value)
        {
            Fail(message);
        }
    }

    public static void False(bool value, string message = "Expected False!")
    {
        if (value)
        {
            Fail(message);
        }
    }

    public static void Null(object obj, string message = "Expected Null!")
    {
        if (obj != null)
        {
            Fail(message);
        }
    }
}