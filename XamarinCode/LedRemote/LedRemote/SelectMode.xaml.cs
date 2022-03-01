using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace LedRemote
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectMode : ContentPage
    {
        public SelectMode()
        {
            InitializeComponent();
            updateTheme();
            animateStatickRainbow();
            animateHueRainbow();
            
        }

        void updateTheme()
        {
            ColorTypeConverter converter = new ColorTypeConverter();

            headerFrame.BackgroundColor = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
            BgG1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color3", "#04CCF5"));
            BgG2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
        }

        private void Mode0Button_Pressed(object sender, EventArgs e)
        {
            Preferences.Set("Mode", 0);
            goBack();
        }
        private void Mode1Button_Pressed(object sender, EventArgs e)
        {
            Preferences.Set("Mode", 1);
            goBack();
        }
        private void Mode2Button_Pressed(object sender, EventArgs e)
        {
            Preferences.Set("Mode", 2);
            goBack();
        }
        private void Mode3Button_Pressed(object sender, EventArgs e)
        {
            Preferences.Set("Mode", 3);
            goBack();
        }
        private void BackButton_Pressed(object sender, EventArgs e)
        {
            goBack();
        }
        void goBack()
        {
            Preferences.Set("Poped", 1);
            this.Navigation.PopModalAsync();
        }

        async void animateStatickRainbow()
        {
            int r, g, b;
            float i = 0;
            float speed = 1f;
            ColorTypeConverter converter = new ColorTypeConverter();
            while (true)
            {
                HsvToRgb(i, 1, 1, out r, out g, out b);

                string hex = "#" + BinHex(r) + BinHex(g) + BinHex(b);
                Mode0Button.BackgroundColor = (Color)converter.ConvertFromInvariantString(hex);
                if (i >= 360) { i = 0; }
                i = i + speed;
                await Task.Delay(10);
            }
        }
        async void animateHueRainbow()
        {
            int r, g, b;
            float s1 = 0, s2 = 360 / 5, s3 = 2 * 360 / 5, s4 = 3 * 360 / 5, s5 = 4 * 360 / 5;
            float speed = 1f;
            ColorTypeConverter converter = new ColorTypeConverter();
            while (true)
            {
                HsvToRgb(s1, 1, 1, out r, out g, out b);
                string hex = "#" + BinHex(r) + BinHex(g) + BinHex(b);
                G1.Color = (Color)converter.ConvertFromInvariantString(hex);

                HsvToRgb(s2, 1, 1, out r, out g, out b);
                hex = "#" + BinHex(r) + BinHex(g) + BinHex(b);
                G2.Color = (Color)converter.ConvertFromInvariantString(hex);

                HsvToRgb(s3, 1, 1, out r, out g, out b);
                hex = "#" + BinHex(r) + BinHex(g) + BinHex(b);
                G3.Color = (Color)converter.ConvertFromInvariantString(hex);

                HsvToRgb(s4, 1, 1, out r, out g, out b);
                hex = "#" + BinHex(r) + BinHex(g) + BinHex(b);
                G4.Color = (Color)converter.ConvertFromInvariantString(hex);

                HsvToRgb(s5, 1, 1, out r, out g, out b);
                hex = "#" + BinHex(r) + BinHex(g) + BinHex(b);
                G5.Color = (Color)converter.ConvertFromInvariantString(hex);

                if (s1 >= 360) { s1 = 0; }
                s1 = s1 + speed;
                if (s2 >= 360) { s2 = 0; }
                s2 = s2 + speed;
                if (s3 >= 360) { s3 = 0; }
                s3 = s3 + speed;
                if (s4 >= 360) { s4 = 0; }
                s4 = s4 + speed;
                if (s5 >= 360) { s5 = 0; }
                s5 = s5 + speed;
                await Task.Delay(10);
            }

        }


        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
        public static string BinHex(int bin)
        {
            if (bin > 255) { bin = 255; }
            string s, e;
            int i;
            i = bin % 16;
            switch (i)
            {
                case 1: s = "1"; break;
                case 2: s = "2"; break;
                case 3: s = "3"; break;
                case 4: s = "4"; break;
                case 5: s = "5"; break;
                case 6: s = "6"; break;
                case 7: s = "7"; break;
                case 8: s = "8"; break;
                case 9: s = "9"; break;
                case 10: s = "A"; break;
                case 11: s = "B"; break;
                case 12: s = "C"; break;
                case 13: s = "D"; break;
                case 14: s = "E"; break;
                case 15: s = "F"; break;
                default: s = "0"; break;
            }

            bin = bin / 16;
            i = bin % 16;
            switch (i)
            {
                case 1: e = "1"; break;
                case 2: e = "2"; break;
                case 3: e = "3"; break;
                case 4: e = "4"; break;
                case 5: e = "5"; break;
                case 6: e = "6"; break;
                case 7: e = "7"; break;
                case 8: e = "8"; break;
                case 9: e = "9"; break;
                case 10: e = "A"; break;
                case 11: e = "B"; break;
                case 12: e = "C"; break;
                case 13: e = "D"; break;
                case 14: e = "E"; break;
                case 15: e = "F"; break;
                default: e = "0"; break;
            }

            return e + s;
        }

    }
}