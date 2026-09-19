using System.Collections.Generic;
using Eto.Forms;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.UX.Controls.Generic;

namespace OpenTabletDriver.UX.Controls.Bindings
{
    public sealed class PenBindingEditor : BindingEditor
    {
        public PenBindingEditor()
        {
            this.Content = new Scrollable
            {
                Border = BorderType.None,
                Content = new StackLayout
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Items =
                    {
                        new TableLayout
                        {
                            Rows =
                            {
                                new TableRow
                                {
                                    Cells =
                                    {
                                        new Group
                                        {
                                            Content = new StackLayout
                                            {
                                                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                                Spacing = 5,
                                                Items =
                                                {
                                                    new Group
                                                    {
                                                        Orientation = Orientation.Horizontal,                                                        ExpandContent = false,                                                        Content = tipButton = new BindingDisplay()
                                                    }.Localize(c => { c.Text = Language.T("Tip Binding"); }),
                                                    new UnitGroup
                                                    {
                                                        Orientation = Orientation.Horizontal,                                                        Content = tipThreshold = new FloatSlider(),                                                        Unit = "%"
                                                    }.Localize(c => { c.Text = Language.T("Tip Threshold"); c.ToolTip = Language.T("The minimum threshold in order for the assigned binding to activate."); })
                                                }
                                            }
                                        }.Localize(c => { c.Text = Language.T("Tip Settings"); }),
                                        new Group
                                        {
                                            Content = new StackLayout
                                            {
                                                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                                Spacing = 5,
                                                Items =
                                                {
                                                    new Group
                                                    {
                                                        ExpandContent = false,                                                        Orientation = Orientation.Horizontal,                                                        Content = eraserButton = new BindingDisplay()
                                                    }.Localize(c => { c.Text = Language.T("Eraser Binding"); }),
                                                    new UnitGroup
                                                    {
                                                        Orientation = Orientation.Horizontal,                                                        Content = eraserThreshold = new FloatSlider(),                                                        Unit = "%"
                                                    }.Localize(c => { c.Text = Language.T("Eraser Threshold"); c.ToolTip = Language.T("The minimum threshold in order for the assigned binding to activate."); })
                                                }
                                            }
                                        }.Localize(c => { c.Text = Language.T("Eraser Settings"); })
                                    }
                                }
                            }
                        },
                        new Group
                        {
                            Content = penButtons = new BindingDisplayList
                            {
                                Prefix = "Pen Binding"
                            }
                        }.Localize(c => { c.Text = Language.T("Pen Buttons"); }),
                        new Group {
                            Content = new StackLayout {
                                Orientation = Orientation.Horizontal,
                                Items = {
                                    new Group {
                                        Orientation = Orientation.Horizontal,                                        Content = disablePressure = new CheckBox {
                                        }.Localize(c => { c.Text = Language.T("Disable Pressure"); })
                                    }.Localize(c => { c.ToolTip = Language.T("Disable pressure if it is available"); }),
                                    new Group {
                                        Orientation = Orientation.Horizontal,                                        Content = disableTilt = new CheckBox {
                                        }.Localize(c => { c.Text = Language.T("Disable Tilt"); })
                                    }.Localize(c => { c.ToolTip = Language.T("Disable tilt if it is available"); }),
                                    new Group {
                                        Orientation = Orientation.Horizontal,                                        Content = disableRotation = new CheckBox {
                                        }.Localize(c => { c.Text = Language.T("Disable Rotation"); })
                                    }.Localize(c => { c.ToolTip = Language.T("Disable rotation if it is available"); }),
                                    new Group {
                                        Orientation = Orientation.Horizontal,                                        Content = enableDragBindings = new CheckBox {
                                        }.Localize(c => { c.Text = Language.T("Drag Bindings"); })
                                    }.Localize(c => { c.ToolTip = Language.T("Pen Bindings require pressure to activate"); }),
                                }
                            }
                        }.Localize(c => { c.Text = Language.T("Miscellaneous"); })
                    }
                }
            };

            tipButton.StoreBinding.Bind(SettingsBinding.Child(c => c.TipButton));
            eraserButton.StoreBinding.Bind(SettingsBinding.Child(c => c.EraserButton));
            tipThreshold.ValueBinding.Bind(SettingsBinding.Child(c => c.TipActivationThreshold));
            eraserThreshold.ValueBinding.Bind(SettingsBinding.Child(c => c.EraserActivationThreshold));
            penButtons.ItemSourceBinding.Bind(SettingsBinding.Child(c => (IList<PluginSettingStore>)c.PenButtons)!);
            disablePressure.CheckedBinding.Cast<bool>().Bind(SettingsBinding.Child(c => c.DisablePressure));
            disableTilt.CheckedBinding.Cast<bool>().Bind(SettingsBinding.Child(c => c.DisableTilt));
            disableRotation.CheckedBinding.Cast<bool>().Bind(SettingsBinding.Child(c => c.DisableRotation));
            enableDragBindings.CheckedBinding.Cast<bool>().Bind(SettingsBinding.Child(c => c.EnableDragBindings));
        }

        private BindingDisplay tipButton, eraserButton;
        private FloatSlider tipThreshold, eraserThreshold;
        private CheckBox disablePressure, disableTilt, disableRotation, enableDragBindings;
        private BindingDisplayList penButtons;
    }
}
