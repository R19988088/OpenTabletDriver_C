using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Output;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using Xunit;

namespace OpenTabletDriver.Tests
{
    public class LineStabilizationFilterTest
    {
        [Fact]
        public void SettingsClampWindowsAndResolvePerPenMaximumPressure()
        {
            var settings = new LineStabilizationSettings
            {
                CoordinateWindow = 0,
                PressureWindow = 17,
                MaximumPressureBySerial = new Dictionary<string, float>
                {
                    ["serial:12"] = 0.4f,
                    ["serial:34"] = 0.5f
                }
            };

            Assert.Equal(1, settings.CoordinateWindow);
            Assert.Equal(16, settings.PressureWindow);
            Assert.Equal(0.4f, settings.ResolveMaximumPressure(new ToolReport { Serial = 12, RawToolID = 34 }));
            Assert.Equal(0.76f, settings.ResolveMaximumPressure(new ToolReport { RawToolID = 34 }));
            Assert.Equal(0.76f, settings.ResolveMaximumPressure(new ToolReport()));
            Assert.Contains("serial:12", JsonConvert.SerializeObject(settings));
        }

        [Fact]
        public void DefaultsMatchSaiStabilizer()
        {
            var settings = new LineStabilizationSettings();

            Assert.Equal(2, settings.CoordinateWindow);
            Assert.Equal(5, settings.PressureWindow);
            Assert.Equal(0, settings.DeadZone);
            Assert.Equal(2.1f, settings.Elasticity);
            Assert.Equal(0.76f, settings.DefaultMaximumPressure);
        }

        [Fact]
        public void AveragesCoordinatesAndEmitsOncePerInput()
        {
            var filter = CreateFilter(new LineStabilizationSettings
            {
                CoordinateWindow = 2,
                PressureWindow = 1,
                DefaultMaximumPressure = 1,
                Elasticity = 1
            }, out var emitted);

            filter.Consume(Report(0, 0, 1000));
            filter.Consume(Report(20, 0, 1000));

            Assert.Equal(PipelinePosition.PreTransform, filter.Position);
            Assert.Equal(2, emitted.Count);
            Assert.Equal(Vector2.Zero, ((ITabletReport)emitted[0]).Position);
            Assert.Equal(new Vector2(10, 0), ((ITabletReport)emitted[1]).Position);
        }

        [Fact]
        public void AppliesDeadZoneMaximumPressureAndStandardCurve()
        {
            var filter = CreateFilter(new LineStabilizationSettings
            {
                CoordinateWindow = 2,
                PressureWindow = 1,
                DeadZone = 0.1f,
                DefaultMaximumPressure = 0.5f,
                Elasticity = 2.1f
            }, out var emitted);

            filter.Consume(Report(1, 1, 300));
            filter.Consume(Report(2, 2, 100));
            filter.Consume(Report(3, 3, 500));

            Assert.Equal(1u, ((ITabletReport)emitted[0]).Pressure);
            Assert.Equal(0u, ((ITabletReport)emitted[1]).Pressure);
            Assert.Equal(1000u, ((ITabletReport)emitted[2]).Pressure);
            Assert.Equal(new Vector2(2.5f, 2.5f), ((ITabletReport)emitted[2]).Position);
        }

        [Fact]
        public void SerialChangesOnlyMaximumPressureAndOutOfRangeRestoresFallback()
        {
            var settings = new LineStabilizationSettings
            {
                CoordinateWindow = 1,
                PressureWindow = 1,
                DefaultMaximumPressure = 1,
                MaximumPressureBySerial = new Dictionary<string, float> { ["serial:9"] = 0.5f },
                Elasticity = 1
            };
            var filter = CreateFilter(settings, out var emitted);

            filter.Consume(new ToolReport { Serial = 9 });
            filter.Consume(Report(0, 0, 500));
            filter.Consume(Report(0, 0, 500));
            filter.Consume(new OutOfRangeReport());
            filter.Consume(Report(0, 0, 500));
            filter.Consume(Report(0, 0, 500));

            Assert.Equal(1000u, emitted.OfType<ITabletReport>().Skip(1).First().Pressure);
            Assert.InRange(emitted.OfType<ITabletReport>().Last().Pressure, 546u, 548u);
            Assert.Equal(1, settings.CoordinateWindow);
            Assert.Equal(1, settings.PressureWindow);
        }

        [Fact]
        public void ZeroPressureClearsStrokeButKeepsToolIdentity()
        {
            var settings = new LineStabilizationSettings
            {
                CoordinateWindow = 2,
                PressureWindow = 1,
                DefaultMaximumPressure = 1,
                MaximumPressureBySerial = new Dictionary<string, float> { ["serial:7"] = 0.5f },
                Elasticity = 1
            };
            var filter = CreateFilter(settings, out var emitted);

            filter.Consume(new ToolReport { Serial = 7 });
            filter.Consume(Report(0, 0, 500));
            filter.Consume(Report(20, 0, 0));
            filter.Consume(Report(100, 0, 500));
            filter.Consume(Report(110, 0, 500));

            var reports = emitted.OfType<ITabletReport>().ToArray();
            Assert.Equal(0u, reports[1].Pressure);
            Assert.Equal(new Vector2(20, 0), reports[1].Position);
            Assert.Equal(new Vector2(100, 0), reports[^2].Position);
            Assert.Equal(1000u, reports[^1].Pressure);
        }

        [Fact]
        public void SamePositionAndPressureStillEmitsButtonChanges()
        {
            var filter = CreateFilter(new LineStabilizationSettings { CoordinateWindow = 2 }, out var emitted);

            filter.Consume(Report(4, 4, 10, false));
            filter.Consume(Report(4, 4, 10, true));

            Assert.Equal(2, emitted.Count);
            Assert.False(((ITabletReport)emitted[0]).PenButtons[0]);
            Assert.True(((ITabletReport)emitted[1]).PenButtons[0]);
        }

        private static LineStabilizationFilter CreateFilter(LineStabilizationSettings settings, out List<IDeviceReport> emitted)
        {
            var result = new List<IDeviceReport>();
            var filter = new LineStabilizationFilter(settings, 1000);
            filter.Emit += report =>
            {
                if (report != null)
                    result.Add(report);
            };
            emitted = result;
            return filter;
        }

        private static TabletReport Report(float x, float y, uint pressure, bool button = false) => new TabletReport
        {
            Raw = [],
            Position = new Vector2(x, y),
            Pressure = pressure,
            PenButtons = [button]
        };

        private sealed class ToolReport : IToolReport
        {
            public byte[] Raw { get; set; } = [];
            public ulong Serial { get; set; }
            public uint RawToolID { get; set; }
            public ToolType Tool { get; set; }
        }
    }
}
