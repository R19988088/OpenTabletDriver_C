using System;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Desktop.Output
{
    public class LineStabilizationFilter : IPositionedPipelineElement<IDeviceReport>
    {
        private const int PressureScale = 65535;
        private const int PressureFloor = 66;

        private readonly LineStabilizationSettings settings;
        private readonly uint maxPenPressure;
        private readonly Average16 xFilter = new Average16();
        private readonly Average16 yFilter = new Average16();
        private readonly Average16 pressureFilter = new Average16();
        private IToolReport? currentTool;
        private bool pressureActive;

        public LineStabilizationFilter(LineStabilizationSettings settings, uint maxPenPressure)
        {
            this.settings = settings;
            this.maxPenPressure = maxPenPressure;
        }

        public event Action<IDeviceReport?>? Emit;

        public PipelinePosition Position => PipelinePosition.PreTransform;

        public void Consume(IDeviceReport? report)
        {
            if (report is IToolReport tool)
            {
                currentTool = tool;
                ClearStroke();
            }
            else if (report is OutOfRangeReport)
            {
                currentTool = null;
                ClearStroke();
            }
            else if (report is ITabletReport tablet)
            {
                if (tablet.Pressure == 0)
                    ClearStroke();
                else
                    Filter(tablet);
            }

            Emit?.Invoke(report);
        }

        private void Filter(ITabletReport report)
        {
            var maximum = settings.ResolveMaximumPressure(currentTool);
            var configuredMaximum = Math.Max(1, (int)MathF.Round(maximum * PressureScale));
            var configuredDeadZone = Math.Max(0, (int)MathF.Round(settings.DeadZone * PressureScale));
            var maximum16 = Math.Max(configuredMaximum, configuredDeadZone + 1);
            var raw16 = maxPenPressure == 0
                ? 0
                : Math.Min(maximum16, (int)Math.Round(report.Pressure * (double)PressureScale / maxPenPressure));
            var normalized = raw16 <= configuredDeadZone
                ? 0
                : (int)Math.Min(PressureScale, (long)(raw16 - configuredDeadZone) * PressureScale / (maximum16 - configuredDeadZone));

            if (normalized == 0)
            {
                report.Pressure = 0;
            }
            else
            {
                var curved = StandardPressureCurve(normalized, settings.Elasticity);
                var filtered = pressureFilter.Apply(curved, settings.PressureWindow);
                var output = pressureActive ? Math.Max(PressureFloor, filtered) : PressureFloor;
                pressureActive = true;
                report.Pressure = (uint)Math.Clamp(
                    Math.Round(output * (double)maxPenPressure / PressureScale),
                    1,
                    maxPenPressure);
            }

            var x = xFilter.Apply((int)MathF.Round(report.Position.X * 256), settings.CoordinateWindow);
            var y = yFilter.Apply((int)MathF.Round(report.Position.Y * 256), settings.CoordinateWindow);
            report.Position = new System.Numerics.Vector2(x / 256f, y / 256f);
        }

        private void ClearStroke()
        {
            xFilter.Clear();
            yFilter.Clear();
            pressureFilter.Clear();
            pressureActive = false;
        }

        private static int StandardPressureCurve(int value, float elasticity)
        {
            if (value <= 0 || value >= PressureScale)
                return Math.Clamp(value, 0, PressureScale);

            var x = value / (double)PressureScale;
            var rise = Math.Pow(x, 0.92);
            var fall = elasticity * Math.Pow(1 - x, 1.19);
            return (int)Math.Round(rise / (rise + fall) * PressureScale);
        }

        private sealed class Average16
        {
            private readonly int[] history = new int[16];
            private int index;
            private int count;

            public int Apply(int input, int window)
            {
                var sum = input;
                var used = 1;
                var previousCount = Math.Min(Math.Clamp(window, 1, 16) - 1, count);
                for (var offset = 0; offset < previousCount; offset++)
                {
                    sum += history[(index - offset - 1 + 16) & 15];
                    used++;
                }

                history[index] = input;
                index = (index + 1) & 15;
                count = Math.Min(count + 1, 16);
                return (sum + used - 1) / used;
            }

            public void Clear()
            {
                index = 0;
                count = 0;
            }
        }
    }
}
