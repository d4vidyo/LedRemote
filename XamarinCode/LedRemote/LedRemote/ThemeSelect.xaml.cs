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
    public partial class ThemeSelect : ContentPage
    {
        public ThemeSelect()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            updateView();
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

        void updateView()
        {

            ColorTypeConverter converter = new ColorTypeConverter();
            headerFrame.BackgroundColor = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
            BgG1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color3", "#04CCF5"));
            BgG2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
            VlG1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color5", "#04CCF5"));
            VlG2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
            BrG1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color5", "#04CCF5"));
            BrG2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
            DaG1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color5", "#04CCF5"));
            DaG2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));

            DbG1.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color5", "#04CCF5"));
            DbG2.Color = (Color)converter.ConvertFromInvariantString(Preferences.Get("Color1", "#195EFF"));
        }

        void setTheme(int Mode)
        {
            switch (Mode)
            {
                case 0:
                    Preferences.Set("Debug", 1);
                    Preferences.Set("Theme", 0);
                    Preferences.Set("Color1", "#195EFF");
                    Preferences.Set("Color2", "#04CCF5");
                    Preferences.Set("Color3", "#04CCF5");
                    Preferences.Set("Color4", "#04CCF5");
                    Preferences.Set("Color5", "#04CCF5");
                    Preferences.Set("ON", "ONGreen");
                    Preferences.Set("OFF", "OFFRed");
                    break;
                case 1:
                    Preferences.Set("Debug", 0);
                    Preferences.Set("Theme", 1);
                    Preferences.Set("Color1", "#350061");
                    Preferences.Set("Color2", "#9D45E6");
                    Preferences.Set("Color3", "#7B00E0");
                    Preferences.Set("Color4", "#421D61");
                    Preferences.Set("Color5", "#5F00AD");
                    Preferences.Set("ON", "ONViolet");
                    Preferences.Set("OFF", "OFFRed");

                    break;
                case 2:
                    Preferences.Set("Debug", 0);
                    Preferences.Set("Theme", 2);
                    Preferences.Set("Color1", "#612400");
                    Preferences.Set("Color2", "#E68045");
                    Preferences.Set("Color3", "#E05401");
                    Preferences.Set("Color4", "#61361D");
                    Preferences.Set("Color5", "#AD4000");
                    Preferences.Set("ON", "ONBrown");
                    Preferences.Set("OFF", "OFFRed");

                    break;
                case 3:
                    Preferences.Set("Debug", 0);
                    Preferences.Set("Theme", 3);
                    Preferences.Set("Color1", "#191919");
                    Preferences.Set("Color2", "#aaaaaa");
                    Preferences.Set("Color3", "#343434");
                    Preferences.Set("Color4", "#2b2b2b");
                    Preferences.Set("Color5", "#343434");
                    Preferences.Set("ON", "ONGreen");
                    Preferences.Set("OFF", "OFFRed");

                    break;
            }
            updateView();
        }

        private void Violet_Pressed(object sender, EventArgs e)
        {
            setTheme(1);
        }
        private void Brown_Pressed(object sender, EventArgs e)
        {
            setTheme(2);
        }
        private void Dark_Pressed(object sender, EventArgs e)
        {
            setTheme(3);
        }

        private void Debug_Pressed(object sender, EventArgs e)
        {
            setTheme(0);
        }

    }
}