using Newtonsoft.Json;
using OpenTabletDriver.Desktop;
using Xunit;

namespace OpenTabletDriver.Tests
{
    public class LineStabilizationSettingsTest
    {
        [Fact]
        public void SettingsRoundTripGlobalAndPerPenValues()
        {
            var settings = new Settings
            {
                LineStabilization = new LineStabilizationSettings
                {
                    CoordinateWindow = 6,
                    PressureWindow = 3,
                    DeadZone = 0.08f,
                    Elasticity = 4.2f,
                    DefaultMaximumPressure = 0.9f,
                    MaximumPressureBySerial = { ["serial:42"] = 0.71f }
                }
            };

            var restored = JsonConvert.DeserializeObject<Settings>(JsonConvert.SerializeObject(settings))!;

            Assert.Equal(6, restored.LineStabilization.CoordinateWindow);
            Assert.Equal(3, restored.LineStabilization.PressureWindow);
            Assert.Equal(0.08f, restored.LineStabilization.DeadZone);
            Assert.Equal(4.2f, restored.LineStabilization.Elasticity);
            Assert.Equal(0.9f, restored.LineStabilization.DefaultMaximumPressure);
            Assert.Equal(0.71f, restored.LineStabilization.MaximumPressureBySerial["serial:42"]);
        }
    }
}
