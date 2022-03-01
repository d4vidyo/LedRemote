using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net;
using System.Net.Http;

namespace LedRemote
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class controlPage : ContentPage
    {
        const int ModeCount = 4;
        int loaded = 1;
        int On = 0;
        int Mode = 0;
        int Theme = 0; 
        float Brightness = 0f;

        string UriOfEsp = "http://192.168.2.69/";
        float[] SpeedVar = new float[ModeCount];
        float[] SquishVar = new float[ModeCount];
        System.Globalization.CultureInfo stringToFloatSettings = System.Globalization.CultureInfo.InvariantCulture;


        public controlPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            Overlay.IsVisible = true;
            loadPresets();
            updateUI();
            updateTheme();
            sendRequest("Status");
            refresh();
        }

        void loadPresets()
        {
            Mode = Preferences.Get("Mode", 0);
            Brightness = Preferences.Get("Brightness", 0f);
            SpeedVar[0] = Preferences.Get("stSpeed", 0f);
            SpeedVar[1] = Preferences.Get("hueSpeed", 0f);
            SquishVar[1] = Preferences.Get("hueSquish", 0f);
            UriOfEsp = "http://" + Preferences.Get("IP", "192.168.2.69") + "/";
        }

        //Wifi communication:
        async void resendLastState()
        {
            await sendRequest("Mode" + Mode.ToString());
            await sendRequest("Brightness" + Brightness.ToString());
            await sendRequest("Speed" + SpeedVar[Mode].ToString());
            await sendRequest("Squish" + SquishVar[Mode].ToString());
        }
        void interpreteLine(String line)
        {
            http.Text += line;
            if (line.Contains("Connected")) { }
            if (line.Contains("Loaded"))
            {
                loaded = int.Parse(line.Substring(line.IndexOf(" ")));
                if (loaded == 0) { resendLastState(); }
            }
            if (line.Contains("On")) { On = int.Parse(line.Substring(line.IndexOf(" "))); }
            if (line.Contains("Mode")) { Mode = int.Parse(line.Substring(line.IndexOf(" "))); }
            if (line.Contains("Brightness")) { Brightness = float.Parse(line.Substring(line.IndexOf(" ")), System.Globalization.NumberStyles.Any); }
            if (line.Contains("Speed")) { SpeedVar[Mode] = float.Parse(line.Substring(line.IndexOf(" ")), stringToFloatSettings); }
            if (line.Contains("Squish")) { SquishVar[Mode] = float.Parse(line.Substring(line.IndexOf(" ")), stringToFloatSettings); }
        }
        void interpreteRequest(String request)
        {
            http.Text = "";
            int indexReturn;
            int indexEnd;
            do
            {
                indexEnd = request.LastIndexOf("\n");               //Get length of request
                indexReturn = request.IndexOf("\n");                //Get length of line
                String line = request.Substring(0, indexReturn);    //Save line 
                request = request.Remove(0, indexReturn + 1);       //Remove line from request. Also remove \n
                interpreteLine(line);                               //further dicection of line
            } while (indexEnd > indexReturn);
            updateUI();
        }
        async Task sendRequest(string modifier)
        {
            String s;
            using (WebClient client = new WebClient())
            {
                Uri URL = new Uri(UriOfEsp + modifier);
                try
                {
                    s = await client.DownloadStringTaskAsync(URL);
                    interpreteRequest(s);
                    Overlay.IsVisible = false;
                }
                catch (WebException we)
                {
                    s = "Error Connecting to Controller";
                }
            }
        }



        //UI updates:
        void updateUI()
        {
            power(On);
            updateModeView();
            BrightnessLabel.Text = "Brightness: " + Brightness.ToString();
            BrightnessSlider.Value = Brightness;

            SpeedLabelStRb.Text = "Speed: " + Math.Round(SpeedVar[0], 2).ToString();
            SpeedLabelHueRb.Text = "Speed: " + Math.Round(SpeedVar[1], 2).ToString();
            SpeedLabelFire.Text = "Speed: " + Math.Round(SpeedVar[3], 2).ToString();
            SpeedSliderStRb.Value = SpeedVar[0];
            SpeedSliderHueRb.Value = SpeedVar[1];
            SpeedSliderFire.Value = SpeedVar[3];

            SquishLabelHueRb.Text = "Squish: " + SquishVar[1].ToString();
            SquishSliderHueRb.Value = SquishVar[1];

            ipEntry.Text = Preferences.Get("IP", "192.168.2.69");
        }
        async void refresh()
        {
            while (true)
            {
                detectModeChange();
                detectThemeChange();
                await Task.Delay(10);
            }
        }
        void detectModeChange()
        {
            if (Mode != Preferences.Get("Mode", 0))
            {
                Mode = Preferences.Get("Mode", 0);
                Overlay.IsVisible = true;
                sendRequest("Mode" + Mode.ToString());
                updateModeView();
            }
        }
        void detectThemeChange()
        {
            if (Theme != Preferences.Get("Theme", 0))
            {
                Theme = Preferences.Get("Theme", 0);
                updateTheme();
            }
        }
        void power(int var)
        {
            ColorTypeConverter converter = new ColorTypeConverter();
            switch (var)
            {
                case 0:
                    OnOff.Source = (String)Preferences.Get("OFF", "OFFRed");
                    break;
                case 1:
                    OnOff.Source = (String)Preferences.Get("ON", "ONGreen");
                    break;
            }
        }
        void updateModeView()
        {
            switch (Mode)
            {
                case 0:
                    Mode0.IsVisible = true;
                    Mode1.IsVisible = false;
                    Mode2.IsVisible = false;
                    Mode3.IsVisible = false;
                    break;
                case 1:
                    Mode0.IsVisible = false;
                    Mode1.IsVisible = true;
                    Mode2.IsVisible = false;
                    Mode3.IsVisible = false;
                    break;
                case 2:
                    Mode0.IsVisible = false;
                    Mode1.IsVisible = false;
                    Mode2.IsVisible = true;
                    Mode3.IsVisible = false;
                    break;
                case 3:
                    Mode0.IsVisible = false;
                    Mode1.IsVisible = false;
                    Mode2.IsVisible = false;
                    Mode3.IsVisible = true;
                    break;
            }
            Preferences.Set("Mode", Mode);
        }
        void updateTheme()
        {
            if (Preferences.Get("Debug", 1) == 1)
            {
                DebugField.IsVisible = true;
                ipEntry.IsVisible = true;
            }
            else
            {
                DebugField.IsVisible = false;
                ipEntry.IsVisible = false;
            }
            power(On);

            ColorTypeConverter converter = new ColorTypeConverter();
            headerFrame.BackgroundColor = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
            BgG1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color3", "#04CCF5"));
            BgG2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
            M0G1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color5", "#04CCF5"));
            M0G2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
        }


        //general options:
        private void BrightnessSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Brightness = (float)e.NewValue;
            BrightnessLabel.Text = "Brightness: " + Math.Round(Brightness).ToString();
        }
        private void BrightnessSlider_DragCompleted(object sender, EventArgs e)
        {
            Preferences.Set("Brightness", Brightness);
            sendRequest("Brightness" + Brightness.ToString(stringToFloatSettings));
        }
        private void ipEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            UriOfEsp = "http://" + e.NewTextValue + "/";
            Preferences.Set("IP", e.NewTextValue);
            http.Text = "ip canged to: " + UriOfEsp;
            Overlay.IsVisible = true;
            sendRequest("Ping");
        }
        //StatickRainbow options:
        private void SpeedSliderStRb_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SpeedVar[0] = (float)e.NewValue;
            SpeedLabelStRb.Text = "Speed: " + Math.Round(SpeedVar[0], 2).ToString();
        }
        private void SpeedSliderStRb_DragCompleted(object sender, EventArgs e)
        {
            Preferences.Set("stSpeed", SpeedVar[0]);
            sendRequest("Speed" + SpeedVar[0].ToString(stringToFloatSettings));
        }
        //HueRainbow options:
        private void SpeedSliderHueRb_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SpeedVar[1] = (float)e.NewValue;
            SpeedLabelHueRb.Text = "Speed: " + Math.Round(SpeedVar[1], 2).ToString();
        }
        private void SpeedSliderHueRb_DragCompleted(object sender, EventArgs e)
        {
            Preferences.Set("hueSpeed", SpeedVar[1]);
            sendRequest("Speed" + SpeedVar[1].ToString(stringToFloatSettings));
        }
        private void SquishSliderHueRb_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SquishVar[1] = (float)e.NewValue;
            SquishLabelHueRb.Text = "Squish: " + SquishVar[1].ToString();

        }
        private void SquishSliderHueRb_DragCompleted(object sender, EventArgs e)
        {
            Preferences.Set("hueSquish", SquishVar[1]);
            sendRequest("Squish" + SquishVar[1].ToString(stringToFloatSettings));
        }

        //Buttonpressed handling:
        private void SavePresets_Pressed(object sender, EventArgs e)
        {


            Preferences.Set("Mode", Mode);
            Preferences.Set("Brightness", Brightness);
            switch (Mode)
            {
                case 0:
                    Preferences.Set("stSpeed", SpeedVar[0]);
                    break;
                case 1:
                    Preferences.Set("hueSpeed", SpeedVar[1]);
                    Preferences.Set("hueSquish", SquishVar[1]);
                    break;
                case 2:

                    break;
                case 3:

                    break;
            }
        }
        private void ThemeButton_Pressed(object sender, EventArgs e)
        {
            ThemeSelect modalPage = new ThemeSelect();
            this.Navigation.PushModalAsync(modalPage);
        }
        private void modeSelection_Pressed(object sender, EventArgs e)
        {
            //Mode0: statickRainbow
            //Mode1: hueRainbow
            //Mode2: statickColor
            //Mode3: Fire/Ice
            SelectMode modalPage = new SelectMode();
            this.Navigation.PushModalAsync(modalPage);
        }
        private void OnOff_Pressed(object sender, EventArgs e)
        {
            switch (On)
            {
                case 0:
                    sendRequest("On");
                    On = 1;
                    power(On);
                    break;
                case 1:
                    sendRequest("Off");
                    On = 0;
                    power(On);
                    break;
            }
        }
    }
}