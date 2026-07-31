using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Microsoft.VisualStudio.TestPlatform.TestExecutor;

namespace EM.Hasher.Tests;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class UnitTestApp : Application
{
    private const string LogFileName = "TestResults.txt";
    private const string FailedFileName = "FailedTests.txt";

    private string _logFilePath = string.Empty;
    private Window? _mainWindow;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public UnitTestApp()
    {
        InitializeComponent();
    }    

    /// <summary>
    /// Invoked when the application is launched.
    /// This is a little rudementry, but will do for now.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var cmdArgs = Environment.GetCommandLineArgs();

        if (cmdArgs.Contains("--run"))
        {
            // see if the folder location is provided in cmd args
            var folderLocationFlag = false;
            foreach (var arg in cmdArgs)
            {
                if (arg.StartsWith("--logfolder"))
                {
                    folderLocationFlag = true; // assume next arg will be the folder location

                    continue;
                }

                if (folderLocationFlag)
                {
                    if (Directory.Exists(arg))
                    {
                        _logFilePath = arg;

                        break;
                    }
                }
            }
            
            // Headless test runner path
            _mainWindow = new UnitTestAppWindow();
            _mainWindow.Activate();
            UITestMethodAttribute.DispatcherQueue = _mainWindow.DispatcherQueue;

            var failures = await RunAllTestsAsync(); // your reflection-based runner

            if (failures > 0)
            {
                File.WriteAllText(AppDataLogFile(FailedFileName), $"{failures} tests failed. See {LogFileName} for details.");
                System.Diagnostics.Debug.WriteLine($"{failures} tests failed. See TestResults.txt for details.");
            }

            Environment.Exit(failures);   // exit code = number of failures

            return;
        }

        UnitTestClient.CreateDefaultUI();

        _mainWindow = new UnitTestAppWindow();
        _mainWindow.Activate();

        UITestMethodAttribute.DispatcherQueue = _mainWindow.DispatcherQueue;

        UnitTestClient.Run(Environment.CommandLine);
    }

    private sealed record TestInvocationResult(string? ArgumentsLabel, Exception? Error);

    private async Task<List<TestInvocationResult>> RunTestMethodAsync(MethodInfo method, object instance)
    {
        // A [DataTestMethod] can have one or more [DataRow] attributes, each supplying
        // the arguments for a single invocation. Parameterless [TestMethod] tests have
        // no rows, so we fall back to a single argument-less invocation.
        var dataRows = method.GetCustomAttributes(typeof(DataRowAttribute), true)
            .Cast<DataRowAttribute>()
            .ToList();

        var results = new List<TestInvocationResult>();

        if (dataRows.Count > 0)
        {
            foreach (var dataRow in dataRows)
            {
                results.Add(await RunSingleInvocationAsync(method, instance, dataRow.Data));
            }
        }
        else
        {
            results.Add(await RunSingleInvocationAsync(method, instance, null));
        }

        return results;
    }

    private async Task<TestInvocationResult> RunSingleInvocationAsync(MethodInfo method, object instance, object?[]? parameters)
    {
        var argumentsLabel = parameters != null
            ? FormatArguments(parameters)
            : null;

        try
        {
            await InvokeTestMethodAsync(method, instance, parameters);
            return new TestInvocationResult(argumentsLabel, null);
        }
        catch (Exception ex)
        {
            return new TestInvocationResult(argumentsLabel, ex.InnerException ?? ex);
        }
    }

    private static string FormatArguments(object?[] parameters)
    {
        var formatted = parameters.Select(p => p switch
        {
            null => "null",
            string s => $"\"{s}\"",
            _ => p.ToString()
        });

        return $"({string.Join(", ", formatted)})";
    }

    private async Task InvokeTestMethodAsync(MethodInfo method, object instance, object?[]? parameters)
    {
        if (UITestMethodAttribute.DispatcherQueue != null)
        {
            var tcs = new TaskCompletionSource<object?>();
            UITestMethodAttribute.DispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    await InvokeAndAwaitAsync(method, instance, parameters);
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex.InnerException ?? ex);
                }
            });
            await tcs.Task; // <-- asynchronous wait avoids deadlock
        }
        else
        {
            await InvokeAndAwaitAsync(method, instance, parameters);
        }
    }

    private static async Task InvokeAndAwaitAsync(MethodInfo method, object instance, object?[]? parameters)
    {
        var result = method.Invoke(instance, parameters);

        // Await async test methods so their assertion failures surface as exceptions.
        if (result is Task task)
        {
            await task;
        }
    }

    private string AppDataLogFile(string fileName)
    {
        string logFile;

        if (string.IsNullOrEmpty(fileName))
        {
            logFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "EM.Hasher.Tests",
                fileName);
        }
        else
        {
            logFile = Path.Combine(_logFilePath, fileName);
        }

        var logDir = Path.GetDirectoryName(logFile);

        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir!);
        }

        return logFile;
    }

    private async Task<int> RunAllTestsAsync()
    {
        var logFile = AppDataLogFile(LogFileName);

        if (File.Exists(logFile))
        {
            File.Delete(logFile);
        }

        var failed = 0;
        var passed = 0;

        var assembly = typeof(UnitTestApp).Assembly;

        var testClasses = assembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(TestClassAttribute), true).Length != 0);

        foreach (var testClass in testClasses)
        {
            var instance = Activator.CreateInstance(testClass);
            var testMethods = testClass.GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), true).Any());

            foreach (var method in testMethods)
            {
                try
                {
                    var results = await RunTestMethodAsync(method, instance!);

                    foreach (var result in results)
                    {
                        var testName = $"{testClass.Name}.{method.Name}{result.ArgumentsLabel}";

                        if (result.Error == null)
                        {
                            passed++;
                            File.AppendAllText(logFile, $"[PASS] {testName}\n");
                            System.Diagnostics.Debug.WriteLine($"[PASS] {testName}");
                        }
                        else
                        {
                            failed++;
                            File.AppendAllText(logFile, $"[FAIL] {testName} - {result.Error.Message}\n");
                            System.Diagnostics.Debug.WriteLine($"[FAIL] {testName} - {result.Error.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    File.AppendAllText(logFile, $"[FAIL] {testClass.Name}.{method.Name} - {ex.InnerException?.Message ?? ex.Message}\n");
                    System.Diagnostics.Debug.WriteLine($"[FAIL] {testClass.Name}.{method.Name} - {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }

        File.AppendAllText(logFile, $"Tests complete. Passed: {passed}, Failed: {failed}\n");
        System.Diagnostics.Debug.WriteLine($"Tests complete. Passed: {passed}, Failed: {failed}");

        return failed;
    }
}
