using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SalusApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetLocation();
            ReadNumber();
            ReadPoliceNumber();
            ReadHospitalNumber();

            LocateME();
        }

        private async void LocateME()
        {
            try
            {
                var geolocator = new Geolocator();
                geolocator.DesiredAccuracyInMeters = 100;
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                Geoposition position = await geolocator.GetGeopositionAsync();

                // reverse geocoding
                BasicGeoposition myLocation = new BasicGeoposition
                {
                    Longitude = position.Coordinate.Point.Position.Longitude,
                    Latitude = position.Coordinate.Point.Position.Latitude

                };
                Geopoint pointToReverseGeocode = new Geopoint(myLocation);

                MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

                if (result.Status == MapLocationFinderStatus.Success)
                {
                    if (result.Locations.Count != 0)
                    {
                        // here also it should be checked if there result isn't null and what to do in such a case
                        string country = result.Locations[0].Address.FormattedAddress;
                        txtGPSCoordinate.Text += country.ToString();

                    }
                }
                else
                {
                    txtGPSCoordinate.Text = result.Status.ToString();
                }
            }
            catch (Exception ex)
            {
                txtGPSCoordinate.Text = ex.ToString();
            }
        }

        private async void GetLocation()
        {
            Geolocator geolocator = new Geolocator();

            // Define the accuracy for the location service in meters
            // There is no point to set the accuracy to any value below 5;
            // Also can define the location service accuracy in DesiredAccuracy with the options of Default or High
            geolocator.DesiredAccuracyInMeters = 10;

            Geoposition currentposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromMinutes(5),    // to specify the aging of the cache 
                timeout: TimeSpan.FromSeconds(10));     // to specify the timeout for the getting the location information

            // There is meaningless to obtaion the position value beyond 6 decimal points. 
            // It will ONLY crate unncessary calculation.
            //
            // Obtain the position value from Coordinate.Point.Position, the information in .Coordinate is retiring.
            string latitude = currentposition.Coordinate.Point.Position.Latitude.ToString("0.000000");
            string longitude = currentposition.Coordinate.Point.Position.Longitude.ToString("0.000000");

            txtGPSCoordinate.Text = latitude + "," + longitude;

        }



        private void btnPolice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI("999", "Police");

        }
        private void btnHospital_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI("999", "Ambulance");
        }


        private async void btnSMS_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (txtFamilyContactNo.Text == "")
            {
                MessageDialog dialog = new MessageDialog("Please Enter Phone Number for Settings", "Information");
                await dialog.ShowAsync();
            }
            else
            {
                Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                msg.Body = "Currently I'm in Danger.My Location at" + txtGPSCoordinate.Text;
                msg.Recipients.Add(txtFamilyContactNo.Text);
                // msg.Recipients.Add("10010");
                await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
            }

        }

        private async void btnAlarm_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MediaElement mysong = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("police_alarm.mp3");
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong.SetSource(stream, file.ContentType);
            mysong.Play();
        }

        private void txtPhoneNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["MySetting"] = txtFamilyContactNo.Text;
        }
        private void ReadNumber()
        {

            // var data = localSettings.Values["MySetting"];
            //txtPhoneNum.Text = localSettings.Values["MySetting"];
            Windows.Storage.ApplicationDataContainer localSettings =
       Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("MySetting"))
            {
                txtFamilyContactNo.Text = localSettings.Values["MySetting"].ToString();
            }
        }


        private async void btnAlarm_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            MediaElement mysong = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("police_alarm.mp3");
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong.SetSource(stream, file.ContentType);
            mysong.Stop();
        }

        private void btnPrivacy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage1));
        }

        private void txtPoliceNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["MyPoliceNo"] = txtPoliceNo.Text;
        }
        private void ReadPoliceNumber()
        {

            // var data = localSettings.Values["MySetting"];
            //txtPhoneNum.Text = localSettings.Values["MySetting"];
            Windows.Storage.ApplicationDataContainer localSettings =
       Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("MyPoliceNo"))
            {
                txtFamilyContactNo.Text = localSettings.Values["MyPoliceNo"].ToString();
            }
        }

        private void txtAmbulanceNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["MyHospitalNo"] = txtPoliceNo.Text;
        }
        private void ReadHospitalNumber()
        {

            // var data = localSettings.Values["MySetting"];
            //txtPhoneNum.Text = localSettings.Values["MySetting"];
            Windows.Storage.ApplicationDataContainer localSettings =
       Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("MyHospitalNo"))
            {
                txtFamilyContactNo.Text = localSettings.Values["MyHospitalNo"].ToString();
            }
        }

        private void btnGPSSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri("ms-settings:privacy-location");

        }
    }
}