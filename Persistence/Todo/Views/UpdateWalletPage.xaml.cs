using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Janus.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Janus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateWallet : ContentPage
    {

        public UpdateWallet()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtJanusId = -1;
        }

        async void OnUpdateClicked(object sender, EventArgs e)
        {
            var wallet = (Wallet)BindingContext;
            await App.Database.SaveItemAsync(wallet);
            await Navigation.PopAsync();

        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var JanusItem = (Wallet)BindingContext;
            await App.Database.DeleteItemAsync(JanusItem);
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

        async void OnConvertClicked(object sender, EventArgs e)
        {
            var JanusItem = (Wallet)BindingContext;

            //redirect to ConvertSingleWallet
            await Navigation.PushAsync(new ConvertSingleWallet(JanusItem)
            {
                BindingContext = JanusItem as Wallet,
            });
        }
    }
}
