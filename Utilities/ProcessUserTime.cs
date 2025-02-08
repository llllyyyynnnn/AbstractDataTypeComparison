using System.Diagnostics;

namespace AbstractDataTypeComparison.Utilities;

public class ProcessUserTime
{
    public double ElapsedTotalSeconds { get; private set; }

    public void Restart()
    {
        processUserTimeStart = Process.GetCurrentProcess().UserProcessorTime.TotalSeconds;
    }

    public void Stop()
    {
        ElapsedTotalSeconds = Process.GetCurrentProcess().UserProcessorTime.TotalSeconds - processUserTimeStart;
    }

    private double processUserTimeStart;
}