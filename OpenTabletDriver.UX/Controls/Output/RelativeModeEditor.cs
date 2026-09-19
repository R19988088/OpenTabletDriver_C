using System;
using Eto.Forms;
using OpenTabletDriver.Desktop.Profiles;
using OpenTabletDriver.UX.Controls.Generic;
using OpenTabletDriver.UX.Controls.Generic.Text;

namespace OpenTabletDriver.UX.Controls.Output
{
    public class RelativeModeEditor : Panel
    {
        public RelativeModeEditor()
        {
            this.Content = new Group
            {
                Content = new StackLayout
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    Spacing = 5,
                    Items =
                    {
                        new StackLayoutItem(null, true),
                        new UnitGroup
                        {
                            Orientation = Orientation.Horizontal,                            Unit = "px/mm",                            Content = xSens = new FloatNumberBox()
                        }.Localize(c => { c.Text = Language.T("X Sensitivity"); }),
                        new UnitGroup
                        {
                            Orientation = Orientation.Horizontal,                            Unit = "px/mm",                            Content = ySens = new FloatNumberBox()
                        }.Localize(c => { c.Text = Language.T("Y Sensitivity"); }),
                        new UnitGroup
                        {
                            Orientation = Orientation.Horizontal,                            Unit = "°",                            Content = rotation = new FloatNumberBox()
                        }.Localize(c => { c.Text = Language.T("Rotation"); }),
                        new UnitGroup
                        {
                            Orientation = Orientation.Horizontal,                            Unit = "ms",                            Content = resetTime = new FloatNumberBox()
                        }.Localize(c => { c.Text = Language.T("Reset Time"); }),
                        new StackLayoutItem(null, true)
                    }
                }
            }.Localize(c => { c.Text = Language.T("Relative"); });

            xSens.ValueBinding.Bind(SettingsBinding.Child(s => s!.XSensitivity));
            ySens.ValueBinding.Bind(SettingsBinding.Child(s => s!.YSensitivity));
            rotation.ValueBinding.Bind(SettingsBinding.Child(s => s!.RelativeRotation));
            resetTime.ValueBinding.Convert<TimeSpan>(
                c => TimeSpan.FromMilliseconds(c),
                v => (float)v.TotalMilliseconds
            ).Bind(SettingsBinding.Child(s => s!.ResetTime));
        }

        private MaskedTextBox<float> xSens, ySens, rotation, resetTime;

        private RelativeModeSettings? settings;
        public RelativeModeSettings? Settings
        {
            set
            {
                this.settings = value;
                this.OnSettingsChanged();
            }
            get => this.settings;
        }

        public event EventHandler<EventArgs>? SettingsChanged;

        protected virtual void OnSettingsChanged() => SettingsChanged?.Invoke(this, new EventArgs());

        public BindableBinding<RelativeModeEditor, RelativeModeSettings?> SettingsBinding
        {
            get
            {
                return new BindableBinding<RelativeModeEditor, RelativeModeSettings?>(
                    this,
                    c => c.Settings,
                    (c, v) => c.Settings = v,
                    (c, h) => c.SettingsChanged += h,
                    (c, h) => c.SettingsChanged -= h
                );
            }
        }
    }
}
