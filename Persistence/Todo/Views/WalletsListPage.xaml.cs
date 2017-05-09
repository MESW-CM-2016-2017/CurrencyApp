﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using CurrencyApp.API;
using CurrencyApp.Models;
using CurrencyApp.Views;
using Xamarin.Forms;

namespace CurrencyApp
{
    public partial class CurrencyAppListPage : ContentPage
    {
        public CurrencyAppListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtCurrencyAppId = -1;
            listView.ItemsSource = await App.Database.GetItemsAsync();
            refreshLabel.Text = "Last Update: " + await App.DatabaseCurrencies.GetLastUpdateTimeString();

        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CurrencyAppItemPage
            {
                BindingContext = new Wallet()
            });
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
            ((App)App.Current).ResumeAtCurrencyAppId = (e.SelectedItem as Wallet).ID;
            Debug.WriteLine("setting ResumeAtCurrencyAppId = " + (e.SelectedItem as Wallet).ID);
            Wallet w = (e.SelectedItem as Wallet);


            await Navigation.PushAsync(new UpdateWallet
            {
                BindingContext = e.SelectedItem as Wallet,
            });

        }


        async void OnPieChartClicked(object sender, EventArgs e)
        {

        }

        async void OnConvertWallet(object sender, EventArgs e)
        {


            await Navigation.PushAsync(new ConvertWalletPage
            {
            });

        }

    }
}
