using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using System.Net.Http;

namespace LedRemote
{
    public partial class MainPage : ContentPage
    {
        float PublicSlider = 0.5f;
        float PubSpeedSlider;
        float PubSquishSlider;
        int currentMode = 0;
        string UriOfEsp = "http://192.168.2.69/";
        public MainPage()
        {
            InitializeComponent();
            Slider.Value = PublicSlider;
            sendRequest("Status");
            //statusRequest();
            //AnimatePreview();
        }

        void interpreteLine(String line)
        {
            if (line.Contains("Connected")) { }
            if (line.Contains("Loaded")) { }
            if (line.Contains("On")) { }
            if (line.Contains("Mode")) { }
            if (line.Contains("Brightness")) { }
            if (line.Contains("Speed")) { }
            if (line.Contains("Squish")) { }
            if (line.Contains("")) { }
            if (line.Contains("")) { }
            if (line.Contains("")) { }
        }
        void interpreteRequest(String request)
        {
            int indexReturn;
            int indexEnd;
            http.Text = "";
            do 
            {
                indexEnd = request.LastIndexOf("\n");               //Get length of request
                indexReturn = request.IndexOf("\n");                //Get length of line
                String line = request.Substring(0, indexReturn);    //Save line 
                request = request.Remove(0, indexReturn + 1);       //Remove line from request. Also remove \n
                interpreteLine(line);                               //further dicection of line
            } while (indexEnd > indexReturn);

        }
        async void sendRequest(string modifier)
        {
            String s;
            using (WebClient client = new WebClient())
            {
                Uri URL = new Uri(UriOfEsp + modifier);
                try
                {
                    s = await client.DownloadStringTaskAsync(URL);
                    interpreteRequest(s);
                }
                catch (WebException we)
                {
                    s = "Error Connecting to Controller";
                }
            }
        }



        private void On_Pressed(object sender, EventArgs e)
        {
            htmlRequest("On");
            power(1);
        }
        private void Off_Pressed(object sender, EventArgs e)
        {
            htmlRequest("Off");
            power(0);
        }
        private void Status_Pressed(object sender, EventArgs e)
        {
            statusRequest();
        }
        private void Mode0_Pressed(object sender, EventArgs e)
        {
            htmlRequest("0Mode");
            ModeActive(0);
        }
        private void Mode1_Pressed(object sender, EventArgs e)
        {
            htmlRequest("1Mode");
            ModeActive(1);
        }
        void ModeActive(int var)
        {
            currentMode = var;
            ColorTypeConverter converter = new ColorTypeConverter();
            switch (var)
            {
                case 0:
                    M0G1.Color = (Color)converter.ConvertFromInvariantString("#00FF00");
                    M1G1.Color = (Color)converter.ConvertFromInvariantString("#195EFF");
                    break;
                case 1:
                    M1G1.Color = (Color)converter.ConvertFromInvariantString("#00FF00");
                    M0G1.Color = (Color)converter.ConvertFromInvariantString("#195EFF");
                    break;
            }
        }
        void power(int var)
        {
            ColorTypeConverter converter = new ColorTypeConverter();
            switch (var)
            {
                case 0:
                    OnG1.Color = (Color)converter.ConvertFromInvariantString("#195EFF");
                    OffG1.Color = (Color)converter.ConvertFromInvariantString("#FF0000");
                    OffG2.Color = (Color)converter.ConvertFromInvariantString("#FF6F00");
                    break;
                case 1:
                    OnG1.Color = (Color)converter.ConvertFromInvariantString("#00FF00");
                    OffG1.Color = (Color)converter.ConvertFromInvariantString("#195EFF");
                    OffG2.Color = (Color)converter.ConvertFromInvariantString("#04CCF5");
                    break;
            }
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PublicSlider = (float)e.NewValue;
            string sliderValue = String.Format("{0:F2}", PublicSlider);
            label.Text = "Brightness:  " + Math.Round(PublicSlider).ToString();
        }
        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            updateOpacity();
        }
        private void SpeedSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PubSpeedSlider = (float)e.NewValue;
            string sliderValue = String.Format("{0:F2}", PubSpeedSlider);
            SpeedLabel.Text = "Speed: " + Math.Round(PubSpeedSlider).ToString();
        }
        private void SpeedSlider_DragCompleted(object sender, EventArgs e)
        {
            update();
        }
        private void SquishSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PubSquishSlider = (float)e.NewValue;
            string sliderValue = String.Format("{0:F2}", PubSquishSlider);
            SquishLabel.Text = "Squish: " + Math.Round(PubSquishSlider, 1).ToString();

        }
        private void SquishSlider_DragCompleted(object sender, EventArgs e)
        {
            update();
        }
        void updateOpacity()
        {
            string sliderValue = String.Format("{0}", (int)PublicSlider);
            htmlRequest(sliderValue + "Opacity");
        }
        void update()
        {
            string sliderValue = String.Format("{0}", (int)PublicSlider);
            string Speed = String.Format("{0}", (int)PubSpeedSlider);
            string Squish = String.Format("{0}", (int)(PubSquishSlider * 10));

            htmlRequest(sliderValue + "B" + Speed + "S" + Squish + "Q" + "hueRainbow");

        }

        async void htmlRequest(string modifier)
        {

            string s;
            using (WebClient client = new WebClient())
            {
                Uri URL = new Uri("http://192.168.2.69/" + modifier);
                try
                {
                    s = await client.DownloadStringTaskAsync(URL);
                    http.TextColor = Color.Green;
                }
                catch (WebException we)
                {
                    s = "Error Connecting to Controller";
                    http.TextColor = Color.Red;
                    htmlRequest(modifier);
                }
            }
            http.Text = s;
        }
        async void statusRequest()
        {
            string s;
            using (WebClient client = new WebClient())
            {
                Uri URL = new Uri("http://192.168.2.69/Status");
                try
                {
                    s = await client.DownloadStringTaskAsync(URL);
                    http.TextColor = Color.Green;

                    String cpy = s;
                    String line;
                    int nextReturn = cpy.IndexOf("\n");
                    int indexEnd = cpy.IndexOf("End");
                    http.Text = "";

                    while (true)
                    {
                        line = cpy.Substring(0, nextReturn);
                        cpy = cpy.Substring(nextReturn + 1, indexEnd - nextReturn + 2);
                        http.Text += float.Parse(line.Substring(line.IndexOf(" "))).ToString() + "\n";

                        if (line.Contains("On")) { power(int.Parse(line.Substring(line.IndexOf(" ")))); }
                        if (line.Contains("Mode")) { ModeActive(int.Parse(line.Substring(line.IndexOf(" ")))); }
                        if (line.Contains("Brightness"))
                        {
                            float var = float.Parse(line.Substring(line.IndexOf(" "))) / 2.55f;
                            Slider.Value = var;
                            label.Text = "Brightness: " + Math.Round(var).ToString();
                        }
                        if (line.Contains("Speed"))
                        {
                            float var = float.Parse(line.Substring(line.IndexOf(" "))) * 10;
                            SpeedSlider.Value = var;
                            SpeedLabel.Text = "Speed: " + Math.Round(var).ToString();
                        }
                        if (line.Contains("Squish"))
                        {
                            float var = float.Parse(line.Substring(line.IndexOf(" "))) / 10;
                            SquishSlider.Value = var;
                            SquishLabel.Text = "Squish: " + Math.Round(var, 1).ToString();
                        }

                        nextReturn = cpy.IndexOf("\n");
                        indexEnd = cpy.IndexOf("End");
                        if (indexEnd < 0) { break; }
                        if (nextReturn < 0) { nextReturn = indexEnd; }
                    }

                }
                catch (WebException we)
                {
                    http.TextColor = Color.Red;
                    http.Text = "Not Connected";
                }
            }
        }


        async void Fade()
        {
            int r, g, b;
            float i = 0;
            float speed = 1f;
            bool swap = false;
            ColorTypeConverter converter = new ColorTypeConverter();
            while (true)
            {
                HsvToRgb(i, 1, 1, out r, out g, out b);

                string hex = "#" + BinHex(r) + BinHex(g) + BinHex(b);
                header.BackgroundColor = (Color)converter.ConvertFromInvariantString(hex);
                if (i >= 360) { swap = true; }
                if (i <= 0) { swap = false; }
                if (swap) { i = i - speed; }
                else { i = i + speed; }
                await Task.Delay(10);
            }
        }
        async void AnimatePreview()
        {
            Fade();
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
