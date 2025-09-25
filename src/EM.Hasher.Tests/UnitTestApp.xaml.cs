using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EM.Hasher.Tests
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class UnitTestApp : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public UnitTestApp()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var cmdArgs = Environment.GetCommandLineArgs();
            if (cmdArgs.Contains("--run"))
            {
                // Headless test runner path
                m_window = new UnitTestAppWindow();
                m_window.Activate();
                UITestMethodAttribute.DispatcherQueue = m_window.DispatcherQueue;

                int failures = await RunAllTestsAsync(); // your reflection-based runner
                
                Environment.Exit(failures);   // exit code = number of failures

                return;
            }

            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();

            m_window = new UnitTestAppWindow();
            m_window.Activate();

            UITestMethodAttribute.DispatcherQueue = m_window.DispatcherQueue;

            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
        }

        private Window? m_window;

        private async Task RunTestMethodAsync(MethodInfo method, object instance)
        {
            if (UITestMethodAttribute.DispatcherQueue != null)
            {
                var tcs = new TaskCompletionSource<object?>();
                UITestMethodAttribute.DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        method.Invoke(instance, null);
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
                method.Invoke(instance, null);
            }
        }

        private async Task<int> RunAllTestsAsync()
        {
            string logFile = Path.Combine(AppContext.BaseDirectory, "../TestResults.txt");
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            int failures = 0;
            var assembly = typeof(UnitTestApp).Assembly;

            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(TestClassAttribute), true).Any());

            foreach (var testClass in testClasses)
            {
                var instance = Activator.CreateInstance(testClass);
                var testMethods = testClass.GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), true).Any());

                foreach (var method in testMethods)
                {
                    try
                    {
                        await RunTestMethodAsync(method, instance!);
                        File.AppendAllText(logFile, $"[PASS] {testClass.Name}.{method.Name}\n");
                        System.Diagnostics.Debug.WriteLine($"[PASS] {testClass.Name}.{method.Name}");
                        //Console.WriteLine($"[PASS] {testClass.Name}.{method.Name}");
                    }
                    catch (Exception ex)
                    {
                        failures++;
                        File.AppendAllText(logFile, $"[FAIL] {testClass.Name}.{method.Name} - {ex.InnerException?.Message ?? ex.Message}\n");
                        System.Diagnostics.Debug.WriteLine($"[FAIL] {testClass.Name}.{method.Name} - {ex.InnerException?.Message ?? ex.Message}");
                        //Console.WriteLine($"[FAIL] {testClass.Name}.{method.Name} - {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
            }

            File.AppendAllText(logFile, $"Tests complete. Failures: {failures}\n");
            System.Diagnostics.Debug.WriteLine($"Tests complete. Failures: {failures}");
            
            return failures;
        }
    }
}
