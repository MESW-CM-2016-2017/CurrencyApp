using System;
using System.Collections.Generic;
using System.Diagnostics;
using Janus.API;
using Janus.Models;
using Janus.Views;
using Xamarin.Forms;

namespace Janus
{
    public partial class JanusListPage : ContentPage
    {

        private List<Wallet> listOfWallets;
        public JanusListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            updateCurrencies();
            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtJanusId = -1;
            listOfWallets = await App.Database.GetItemsAsync();
            listView.ItemsSource = listOfWallets;
            refreshLabel.Text = "Last Update: " + await App.DatabaseCurrencies.GetLastUpdateTimeString();

        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new JanusItemPage
            {
                BindingContext = new Wallet()
            });
        }

        void updateCurrencies()
        {
            try
            {
                APIHandler api = new APIHandler();
                refreshLabel.Text = api.UpdateCurrencies();
            }
            catch (Exception exception)
            {
                // show dialog
                //await DisplayAlert("Warning", "The API is currently unavailable. Please Try Again", "OK");
            }
        }

        async void OnRefresh(object sender, EventArgs e)
        {
            if (Utils.Utils.hasInternetAccess())
            {
                // atualizar currencies
                try
                {
                    APIHandler api = new APIHandler();
                    refreshLabel.Text = api.UpdateCurrencies();
                }
                catch (Exception exception)
                {
                    // show dialog
                    //await DisplayAlert("Warning", "The API is currently unavailable. Please Try Again", "OK");
                }
                // mudar label
            }
            else
            {
                // no internet show access
                String heho = "No Internet";
                await DisplayAlert("Warning", "No Internet Access. Please Connect to the Internet.", "OK");

            }

        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((App)App.Current).ResumeAtJanusId = (e.SelectedItem as Wallet).ID;
            Debug.WriteLine("setting ResumeAtJanusId = " + (e.SelectedItem as Wallet).ID);
            Wallet w = (e.SelectedItem as Wallet);


            await Navigation.PushAsync(new UpdateWallet
            {
                BindingContext = e.SelectedItem as Wallet,
            });

        }

        async void OnConvertWallet(object sender, EventArgs e)
        {
            if (listOfWallets.Capacity != 0)
            {
                await Navigation.PushAsync(new ConvertWalletPage(listOfWallets)
                {

                });
            }
            else
            {
                await DisplayAlert("Warning", "There are no wallets to convert.", "OK");
            }

        }

    }
}
