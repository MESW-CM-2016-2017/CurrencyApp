using System;
using System.Collections.Generic;
using Janus.Models;
using Xamarin.Forms;

namespace Janus
{
	public partial class JanusItemPage : ContentPage
	{
		public JanusItemPage()
		{
			InitializeComponent();
            foreach (String symbol in CurrencyDTO.top10Currencies)
            {
                if (Currency.SelectedIndex != 0) Currency.SelectedIndex = 0;
                Currency.Items.Add(symbol);
            }
        }

		async void OnSaveClicked(object sender, EventArgs e)
		{
            bool entered = false;
            var JanusItem = (Wallet)BindingContext;
            List<Wallet> wallets = await App.Database.GetItemsAsync();
            Wallet wallet = new Wallet();
            if (wallets.Count > 0)
            {
                foreach (Wallet w in wallets)
                {
                    if (w.Symbol.Equals(JanusItem.Symbol))
                    {
                        wallet = w;
                        entered = true;
                        break;
                    }
                }
                if (entered == true)
                {
                    wallet.Quantity = wallet.Quantity + JanusItem.Quantity;
                    await App.Database.SaveItemAsync(wallet);
                }
                else
                {
                    await App.Database.SaveItemAsync(JanusItem);
                }
                
            }
            else
            {
                await App.Database.SaveItemAsync(JanusItem);
            }
            
			await Navigation.PopAsync();
		}

		async void OnCancelClicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync();
		}

		void OnSpeakClicked(object sender, EventArgs e)
		{
			var JanusItem = (Wallet)BindingContext;
			DependencyService.Get<ITextToSpeech>().Speak(JanusItem.Quantity + " " + JanusItem.Symbol);
		}
	}
}
