using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO.Compression;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Registry = Microsoft.Win32.Registry;
using RegistryKey = Microsoft.Win32.RegistryKey;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.AppModel;
using PaintDotNet.IndirectUI;
using PaintDotNet.Collections;
using PaintDotNet.PropertySystem;
using IntSliderControl = System.Int32;
using CheckboxControl = System.Boolean;
using ColorWheelControl = PaintDotNet.ColorBgra;
using AngleControl = System.Double;
using PanSliderControl = PaintDotNet.Pair<double, double>;
using TextboxControl = System.String;
using FilenameControl = System.String;
using DoubleSliderControl = System.Double;
using ListBoxControl = System.Byte;
using RadioButtonControl = System.Byte;
using ReseedButtonControl = System.Byte;
using MultiLineTextboxControl = System.String;
using RollControl = System.Tuple<double, double, double>;


namespace PdnPlugins
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get
            {
                return base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            }
        }

        public string Copyright
        {
            get
            {
                return base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            }
        }

        public string DisplayName
        {
            get
            {
                return base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            }
        }

        public Version Version
        {
            get
            {
                return base.GetType().Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("https://www.getpaint.net/redirect/plugins.html");
            }
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "oSoft GrayScale")]
    public class GrayScalePlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "oSoft GrayScale";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return null;
            }
        }

        public static string SubmenuName
        {
            get
            {
                return null;
            }
        }

        public GrayScalePlugin()
            : base(StaticName, StaticIcon, SubmenuName, new EffectOptions() { Flags = EffectFlags.None })
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();
            return new PropertyCollection(props);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);
            return configUI;
        }

        protected override void OnCustomizeConfigUIWindowProperties(PropertyCollection props)
        {
            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.PlainText;
            props[ControlInfoPropertyNames.WindowHelpContent].Value = "oSoft GrayScale v1.0\nCopyright ©2018 by \nAll rights reserved.";
            base.OnCustomizeConfigUIWindowProperties(props);
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {          
            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            if (length == 0) return;
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Render(DstArgs.Surface, SrcArgs.Surface, rois[i]);
            }
        }

        #region User Entered Code
        // Name:
        // Submenu:
        // Author:
        // Title:
        // Version:
        // Desc:
        // Keywords:
        // URL:
        // Help:
        
        void Render(Surface dst, Surface src, Rectangle rect)
        {
            // Delete any of these lines you don't need
            Rectangle selection = EnvironmentParameters.GetSelection(src.Bounds).GetBoundsInt();
            int CenterX = ((selection.Right - selection.Left) / 2) + selection.Left;
            int CenterY = ((selection.Bottom - selection.Top) / 2) + selection.Top;
            ColorBgra PrimaryColor = EnvironmentParameters.PrimaryColor;
            ColorBgra SecondaryColor = EnvironmentParameters.SecondaryColor;
            int BrushWidth = (int)EnvironmentParameters.BrushWidth;

            ColorBgra CurrentPixel;
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                if (IsCancelRequested) return;
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    CurrentPixel = src[x, y];
                    // TODO: Add pixel processing code here
                    // Access RGBA values this way, for example:
                    // CurrentPixel.R = PrimaryColor.R;
                    // CurrentPixel.G = PrimaryColor.G;
                    // CurrentPixel.B = PrimaryColor.B;
                    // CurrentPixel.A = PrimaryColor.A;
                    var l = CurrentPixel.Luminance();
                    // if (l != CurrentPixel.GetIntensityByte()) Debug.WriteLine($"{l}/{CurrentPixel.GetIntensityByte()}");
                    CurrentPixel.R = l;
                    CurrentPixel.G = l;
                    CurrentPixel.B = l;
                    dst[x, y] = CurrentPixel;
                }
            }
        }

        #endregion
    }
}
