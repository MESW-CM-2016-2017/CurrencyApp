using Janus.API;
using Janus.Models;
using Janus.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Janus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConvertWalletPage : ContentPage
    {
        private Picker Currency;
        private Button Button;
        private StackLayout Chart;

        private List<Wallet> Wallets;

        public ConvertWalletPage(List<Wallet> listOfWallets)
        {
            //InitializeComponent();
            createPicker();
            createButton();
            Wallets = listOfWallets;
            Chart = BarChartFactory.createChart(listOfWallets);
            createPage();
        }

        private void createPage()
        {
            var contentView = new ContentView
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = {
                     Currency,
                     Button,
                     Chart
                 }
                }
            };

            Content = contentView;
        }

        private async void refresh(String selectedSymbol)
        {
            List<Wallet> list = new List<Wallet>();
            foreach(Wallet w in Wallets)
            {
                Double qtd = await APIHandler.ConvertWithoutSaving(selectedSymbol, w, w.Quantity);
                list.Add(new Wallet(qtd, w.Symbol));
            }
            Chart = BarChartFactory.createChart(list);
            try
            {
                createPage();
            }catch(Exception e)
            {
                //
            }
        }

        private void createPicker()
        {
            Currency = new Picker { Title = "Choose" };
            foreach (String symbol in CurrencyDTO.top10Currencies)
            {
                //if (Currency.SelectedIndex != 0) Currency.SelectedIndex = 0;
                Currency.Items.Add(symbol);
            }
            Currency.SelectedIndexChanged += (sender, args) =>
            {
                if (Currency.SelectedIndex != -1)
                {
                    string toSymbol = (string)Currency.SelectedItem;
                    refresh(toSymbol);
                }
            };
        }

        private void createButton()
        {
            Button = new Button { Text = "Convert" };
            Button.Clicked += delegate
            {
                OnConvert();
            };
        }

        async void OnConvert()
        {
            try
            {
                string toSymbol = (string)Currency.SelectedItem;
                //Get Wallets
                List<Wallet> wallets = await App.Database.GetItemsAsync();
                foreach (Wallet wallet in wallets)
                {
                    if(wallet.Symbol.CompareTo(toSymbol) != 0)
                    await APIHandler.Convert(wallet.Symbol, toSymbol, wallet, wallet.Quantity);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            await Navigation.PopToRootAsync();
        }      

    }
}