using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using EasyAuth.Helpers;
using Xamarin.Forms;

namespace EasyAuth
{
    public partial class LoginPage : ContentPage
    {

        string URL = String.Format("https://accounts.spotify.com/authorize/?client_id={0}" +
                                   "&response_type=code&state=34fFs29kd0" +
                                   "&redirect_uri={1}" +
                                   "&scope={2}",
                                   Credentials.ClientID,
                                   Credentials.RedirectURI,
                                   Credentials.Scopes);
        
        public LoginPage()
        {
            InitializeComponent();
            Browser.Source = URL;
            Browser.Navigated += Browser_Navigated;
        }

        async void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {   
            //verify callback URL
            Debug.WriteLine(e.Url);

            Uri url = new Uri(e.Url);

            if (url.Host.Contains("example.com"))
            {
                //parse the response
                var code = HttpUtility.ParseQueryString(url.Query).Get("code");
                var error = HttpUtility.ParseQueryString(url.Query).Get("error");
                //exchange this for a token
                Debug.WriteLine("Got Code: " + code);

                if (error != null)
                {
                    Debug.WriteLine("Error with logging user in");
                    await DisplayAlert("Uh-oh", "There was trouble logging you in", "Try Again");
                    Browser.Source = URL;
      
                    await Navigation.PopAsync();
                }

                //To-do: Exchange the code for an access token
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //Save the RefreshToken and set App AccessToken and LastRefreshedTime
                    var postbackURL = "https://accounts.spotify.com/api/token";
                    var tokens = await OAuth2Helper.GetAccessTokenFromCode(postbackURL,
                           Credentials.RedirectURI,
                           Credentials.ClientID,
                           Credentials.ClientSecret,
                           Credentials.Scopes,
                           code);

                    //In App.cs, add these variables to maintain context
                    App.HasLoggedIn = true;
                    App.AuthModel = tokens;

                    await Navigation.PushModalAsync(new ProfilePage()); 

                });
            }
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
