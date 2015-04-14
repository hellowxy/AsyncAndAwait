using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
/************************
 * The await operator tries to capture synchronization contexts, and executes the following code
 * on it. This helps us write asynchronous code by working with user interface controls. In addition,
 * deadlock situations like those that were described in previous example will not happen when using await,
 * since we do not block the UI thread.
 *************************/
namespace SynchronizationContextDemo
{
    class Program
    {
        private static Label _label;
        [STAThread]
        static void Main(string[] args)
        {
            var app = new Application();
            var win = new Window();
            var panel = new StackPanel();
            var button = new Button();
            _label = new Label();
            _label.FontSize = 32;
            _label.Height = 200;
            button.Height = 100;
            button.FontSize = 32;
            button.Content = new TextBlock() {Text = "Start asynchronous operations"};
            button.Click += Click;
            panel.Children.Add(_label);
            panel.Children.Add(button);
            win.Content = panel;
            app.Run(win);
            Console.Read();
        }

        async static void Click(object sender, EventArgs e)
        {
            _label.Content = new TextBlock() {Text = "Calculating ..."};
            //
            TimeSpan resultWithContext = await Test();
            TimeSpan resultNoContext = await TestNoContext();
            //If we uncomment the next code, when running the application. We will get a multithreaded
            //control access exception. because the code that sets the Label control text will not be 
            //posted on the captured context, but will be executed on a thread pool worker thread instead.
            //TimeSpan resultNoContext = await TestNoContext().ConfigureAwait(false);
            var sb = new StringBuilder();
            sb.AppendFormat("With the context:{0}", resultWithContext);
            sb.AppendLine();
            sb.AppendFormat("Without the context:{0}", resultNoContext);
            sb.AppendLine();
            sb.AppendFormat("Ratio:{0:0.00}", resultWithContext.TotalMilliseconds/resultNoContext.TotalMilliseconds);
            sb.AppendLine();
            _label.Content = new TextBlock() {Text = sb.ToString()};
        }

        async static Task<TimeSpan> Test()
        {
            const int iterationsNumber = 100000;
            var sw = new Stopwatch();
            sw.Start();
            //The regular await operator takes much more time to complete. This is 
            //because we post 100000 continuation tasks on the UI thread, which uses its message loop
            // to asynchronously work with those tasks.
            for (int i = 0; i < iterationsNumber; i++)
            {
                var t = Task.Run(() => { });
                await t;
            }
            sw.Stop();
            return sw.Elapsed;
        }

        async static Task<TimeSpan> TestNoContext()
        {
            const int iterationsNumber = 100000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterationsNumber; i++)
            {
                var t = Task.Run(() => { });
                //The ConfigureAwait method with false as a parameter value, explicitly instructs that
                //we should not use captured synchronization contexts to run continuation code on it.
                //Because we do not access the UI components from the asynchronous operation, using ConfigureAwait
                //with false will be a much more effcient solution.
                await t.ConfigureAwait(continueOnCapturedContext: false);
            }
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
