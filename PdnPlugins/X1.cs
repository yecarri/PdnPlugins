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

[assembly: AssemblyTitle("MyScript4 plugin for paint.net")]
[assembly: AssemblyDescription("MyScript4 selected pixels")]
[assembly: AssemblyConfiguration("myscript4")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("MyScript4")]
[assembly: AssemblyCopyright("Copyright ©2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.*")]

namespace MyScript4Effect
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

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "MyScript4")]
    public class MyScript4EffectPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "MyScript4";
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

        public MyScript4EffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.None)
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return PropertyCollection.CreateEmpty();
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            PreRender(dstArgs.Surface, srcArgs.Surface);

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
        #region UICode
        #endregion

        void PreRender(Surface dst, Surface src)
        {
        }

        // Here is the main render loop function
        void Render(Surface dst, Surface src, Rectangle rect)
        {
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                if (IsCancelRequested) return;
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    ColorBgra CurrentPixel = src[x, y];

                    // TODO: Add additional pixel processing code here



                    dst[x, y] = CurrentPixel;
                }
            }
        }


        #endregion
    }
}
