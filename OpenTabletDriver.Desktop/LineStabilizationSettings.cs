using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Desktop
{
    public class LineStabilizationSettings : ViewModel
    {
        private int coordinateWindow = 2;
        private int pressureWindow = 5;
        private float deadZone;
        private float elasticity = 2.1f;
        private float defaultMaximumPressure = 0.76f;
        private Dictionary<string, float> maximumPressureBySerial = new Dictionary<string, float>();

        [JsonProperty(nameof(CoordinateWindow))]
        public int CoordinateWindow
        {
            get => coordinateWindow;
            set => RaiseAndSetIfChanged(ref coordinateWindow, Math.Clamp(value, 1, 16));
        }

        [JsonProperty(nameof(PressureWindow))]
        public int PressureWindow
        {
            get => pressureWindow;
            set => RaiseAndSetIfChanged(ref pressureWindow, Math.Clamp(value, 1, 16));
        }

        [JsonProperty(nameof(DeadZone))]
        public float DeadZone
        {
            get => deadZone;
            set => RaiseAndSetIfChanged(ref deadZone, Math.Clamp(value, 0, 1));
        }

        [JsonProperty(nameof(Elasticity))]
        public float Elasticity
        {
            get => elasticity;
            set => RaiseAndSetIfChanged(ref elasticity, Math.Clamp(value, 0.1f, 16));
        }

        [JsonProperty(nameof(DefaultMaximumPressure))]
        public float DefaultMaximumPressure
        {
            get => defaultMaximumPressure;
            set => RaiseAndSetIfChanged(ref defaultMaximumPressure, Math.Clamp(value, 0.001f, 1));
        }

        [JsonProperty(nameof(MaximumPressureBySerial))]
        public Dictionary<string, float> MaximumPressureBySerial
        {
            get => maximumPressureBySerial;
            set => RaiseAndSetIfChanged(ref maximumPressureBySerial, value ?? new Dictionary<string, float>());
        }

        public float ResolveMaximumPressure(IToolReport? tool)
        {
            if (tool?.Serial > 0 && MaximumPressureBySerial.TryGetValue($"serial:{tool.Serial}", out var maximum))
                return Math.Clamp(maximum, 0.001f, 1);

            return DefaultMaximumPressure;
        }
    }
}
