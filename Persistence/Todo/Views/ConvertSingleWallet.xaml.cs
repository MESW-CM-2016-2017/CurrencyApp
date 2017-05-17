using Janus.API;
using Janus.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Janus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConvertSingleWallet : ContentPage
    {
        private Double oldQuantity;
        private Wallet Wallet;

        public ConvertSingleWallet(Wallet JanusItem)
        {
            InitializeComponent();
            //var JanusItem = (Wallet)BindingContext;
            setPicker();
            setInput();
            try
            {
                oldQuantity = JanusItem.Quantity;
                Wallet = JanusItem;
                foreach (String symbol in CurrencyDTO.top10Currencies)
                {
                    //if (Picker.SelectedIndex != 0) Picker.SelectedIndex = 0;
                    if (!JanusItem.Symbol.Contains(symbol)) Picker.Items.Add(symbol);
                }

            }
            catch (Exception e)
            {
                var ola = "";
            }

        }


        private void setInput()
        {
            InputQuantity.TextChanged += (sender, args) =>
            {
                if (Picker.SelectedIndex != -1)
                {
                    string toSymbol = (string)Picker.SelectedItem;
                    if (toSymbol != null && toSymbol.Length == 3)
                        refreshLabel(toSymbol, Wallet.Quantity);
                }
            };
        }

        private void setPicker()
        {
            Picker.SelectedIndexChanged += (sender, args) =>
            {
                if (Picker.SelectedIndex != -1)
                {
                    string toSymbol = (string)Picker.SelectedItem;
                    refreshLabel(toSymbol, Wallet.Quantity);
                }
            };
        }

        private async void refreshLabel(String toSymbol, Double quantity)
        {
            Double convertedQuantity = await APIHandler.ConvertWithoutSaving(toSymbol, Wallet, quantity);
            FinalQTD.Text = convertedQuantity.ToString() + " " + toSymbol;
        }

        async void OnConvertClicked(object sender, EventArgs e)
        {
            Wallet JanusItem = (Wallet)BindingContext;
            Double selectedQuantity = JanusItem.Quantity;
            JanusItem.Quantity = oldQuantity;
            string toSymbol = (string)Picker.SelectedItem;
            // convert
            if (selectedQuantity > 0)
            {
                await APIHandler.Convert(JanusItem.Symbol, toSymbol, JanusItem, selectedQuantity);
                await Navigation.PopToRootAsync();
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
                    api.UpdateCurrencies();
                    return;
                }
                catch (Exception exception)
                {
                    // show dialog
                    //await DisplayAlert("Warning", "The API is currently unavailable. Please Try Again", "OK");
                }
            }
            else
            {
                // no internet show access
                await DisplayAlert("Warning", "No Internet Access. Please Connect to the Internet.", "OK");
            }

        }

    }
}
