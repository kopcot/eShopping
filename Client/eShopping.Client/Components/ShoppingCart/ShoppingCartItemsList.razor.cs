using Basket.Core.Entities;
using eShopping.Client.Data;
using eShopping.Client.Pages;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Net;
using System.Net.Mail;
using E = Basket.Core.Entities;
using SS = Shared.Core.Specs;

namespace eShopping.Client.Components.ShoppingCart
{
    public partial class ShoppingCartItemsList : IDisposable
    {
        [Inject]
        private IShoppingCartService ShoppingCartService { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private IErrorService ErrorService { get; set; }
        [Parameter, EditorRequired]
        public int? ShoppingCartId { get; set; }
        private E.ShoppingCart? shoppingCart { get; set; }
        private bool disposedValue;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            HttpStatusCode? httpStatusCode;
            bool result;
            Exception? exception;

            if (ShoppingCartId is not null)
            {
                (result, httpStatusCode, exception, shoppingCart) = await ShoppingCartService.GetShoppingCartByIdAsync<E.ShoppingCart>((int)ShoppingCartId);
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            }
            else
                shoppingCart = null;
        }
        private async Task HandleRowClickShoppingCartsAsync(int productId)
        {
            UriBuilder uriBuilder = new(Navigation.Uri);
            uriBuilder.Query = null;
            uriBuilder.Path += "/" + productId;
            Navigation.NavigateTo(uriBuilder.Uri.AbsoluteUri);
            await Task.CompletedTask;
        }
        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ShoppingCartItemsList()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose
    }
}