namespace Healthcheck.Service.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public static class HardwareUtil
    {
        public static double GetCpuLoadAsync(int MeasurementWindow)
        {
            Process CurrentProcess = Process.GetCurrentProcess();

            TimeSpan StartCpuTime = CurrentProcess.TotalProcessorTime;
            Stopwatch Timer = Stopwatch.StartNew();

            Thread.Sleep(1000);

            TimeSpan EndCpuTime = CurrentProcess.TotalProcessorTime;
            Timer.Stop();

            return (EndCpuTime - StartCpuTime).TotalMilliseconds / (Environment.ProcessorCount * Timer.ElapsedMilliseconds);
        }
    }
}