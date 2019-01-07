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
    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "oSoft Shafait")]
    public class ShafaitPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "oSoft/Shafait";
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
                return "oSoft";
            }
        }

        #region UICode
        IntSliderControl Size = 25; // [1,100] Size
        DoubleSliderControl K = 0.2; // [0.01,0.4] k
        #endregion

        public ShafaitPlugin()
            : base(StaticName, StaticIcon, SubmenuName, new EffectOptions() { Flags = EffectFlags.Configurable })
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property(PropertyNames.Size, 25, 1, 100));
            props.Add(new DoubleProperty(PropertyNames.K, 0.2, 0.01, 0.4));

            return new PropertyCollection(props);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);
            configUI.SetPropertyControlValue(PropertyNames.Size, ControlInfoPropertyNames.DisplayName, "Size");
            configUI.SetPropertyControlValue(PropertyNames.K, ControlInfoPropertyNames.DisplayName, "k");
            return configUI;
        }

        protected override void OnCustomizeConfigUIWindowProperties(PropertyCollection props)
        {
            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.PlainText;
            props[ControlInfoPropertyNames.WindowHelpContent].Value = "oSoft Shafait v1.0\nCopyright ©2018 by \nAll rights reserved.";
            base.OnCustomizeConfigUIWindowProperties(props);
        }

        public enum PropertyNames
        {
            Size,
            K
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Size = newToken.GetProperty<Int32Property>(PropertyNames.Size).Value;
            K = newToken.GetProperty<DoubleProperty>(PropertyNames.K).Value;
            InitializeIntegralImage(srcArgs.Surface);
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


        ulong[,] I = null;
        
        void Render(Surface dst, Surface src, Rectangle rect)
        {

            for (int y = rect.Top; y < rect.Bottom; y++)
            {

                for (int x = rect.Left; x < rect.Right; x++)
                {
                    if (IsCancelRequested) return;


                    var t = Threshold(src, x, y);

                    if (src[x, y].Luminance() > t)
                        dst[x, y] = new ColorBgra() { R = 255, G = 255, B = 255, A = 255 };
                    else
                    {
                        dst[x, y] = new ColorBgra() { R = 0, G = 0, B = 0, A = 255 };                        
                    }
                }
            }
            
        }

        private void InitializeIntegralImage(Surface src)
        {
            I = new ulong[src.Width, src.Height];

            for (int y = 0; y < src.Height; y++)
            {

                for (int x = 0; x < src.Width; x++)
                {
                    if (IsCancelRequested) return;

                    I[x, y] = src[x, y].Luminance() + (y > 0 ? I[x, y - 1] : 0) + (x > 0 ? I[x - 1, y] : 0) - (x > 0 && y > 0 ? I[x - 1, y - 1] : 0);

                }

            }
        }

        public byte LocalMean(int x, int y, int w)
        {
            int top = y - w / 2 < 0 ? 0 : y - w / 2;
            int left = x - w / 2 < 0 ? 0 : x - w / 2;
            int bottom = y + w / 2 > (I.GetLength(1) - 1) ? (I.GetLength(1) - 1) : y + w / 2;
            int right = x + w / 2 > (I.GetLength(0) - 1) ? (I.GetLength(0) - 1) : x + w / 2;

            ulong r = (
                    I[left, top]
                    - I[right, top]
                    - I[left, bottom]
                    + I[right, bottom]) / ((ulong)(bottom - top + 1) * (ulong)(right - left + 1));

            return (byte)r;
        }

        public Int64 LocalVariance(Surface src, int x, int y, int w, byte mean)
        {
            Int64 acc = 0;

            int top = y - w / 2 < 0 ? 0 : y - w / 2;
            int left = x - w / 2 < 0 ? 0 : x - w / 2;
            int bottom = y + w / 2 > src.Height ? src.Height : y + w / 2;
            int right = x + w / 2 > src.Width ? src.Width : x + w / 2;


            for (int i = left; i < right; i++)
                for (int j = top; j < bottom; j++)
                {
                    var intenity = src[i, j].Luminance();
                    acc += (intenity * intenity) - (mean * mean);
                }

            return acc / ((right - left + 1) * (bottom - top + 1));
        }


        public byte Threshold(Surface src, int x, int y)
        {
            const double R = 128;

            var mean = LocalMean(x, y, Size);
            var std = Math.Sqrt(LocalVariance(src, x, y, Size, mean));

            var t = mean * (1 + K * ((std / R) - 1));

            return (byte)t;
        }


        #endregion
    }
}
