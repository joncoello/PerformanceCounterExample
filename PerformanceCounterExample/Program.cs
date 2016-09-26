using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceCounterExample {
    class Program {
        static void Main(string[] args) {

            var bgWk = new BackgroundWorker();
            bgWk.WorkerSupportsCancellation = true;
            bgWk.DoWork += (s, e) => {

                var category = new PerformanceCounterCategory("Network Interface");
                var instances = category.GetInstanceNames();
                var instance = instances.FirstOrDefault(i => i.ToLower().Contains("wireless"));
                var counters = category.GetCounters(instance);

                var c1 = counters.FirstOrDefault(c => c.CounterName == @"Bytes Total/sec");
                var c2 = counters.FirstOrDefault(c => c.CounterName == @"Current Bandwidth");
                
                while (!bgWk.CancellationPending) {

                    Thread.Sleep(1000);

                    var bytesPerSecond = Convert.ToDecimal(c1.RawValue);
                    var bandwidth = Convert.ToDecimal(c2.RawValue);

                    var utilization = bytesPerSecond / bandwidth;

                    Console.WriteLine("Value is at " + utilization.ToString());

                }
                
            };
            bgWk.RunWorkerAsync();

            Console.ReadKey();

            bgWk.CancelAsync();

        }
    }
}
