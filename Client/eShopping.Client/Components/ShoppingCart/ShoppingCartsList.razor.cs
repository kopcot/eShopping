using eShopping.Client.Data;
using eShopping.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Net;
using E = Basket.Core.Entities;
using SS = Shared.Core.Specs;

namespace eShopping.Client.Components.ShoppingCart
{
    public partial class ShoppingCartsList : IDisposable
    {
        [Inject]
        private IShoppingCartService ShoppingCartService { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private IErrorService ErrorService { get; set; }
        [CascadingParameter(Name = "ShoppingCartsPage")]
        public ShoppingCarts ShoppingCartsPage { get; set; }
        public int QueryProductCount
        {
            get => ShoppingCartsPage.QueryProductCount;
            set => ShoppingCartsPage.QueryProductCount = value;
        }

        public int QueryPageIndex
        {
            get => ShoppingCartsPage.QueryPageIndex;
            set => ShoppingCartsPage.QueryPageIndex = value;
        }
        private IEnumerable<E.ShoppingCart>? shoppingCarts;
        public long? shoppingCartsCount { get; set; }
        private readonly List<CancellationTokenSource> _cancellationTokenPageIndexChange = [];
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

            (result, httpStatusCode, exception, shoppingCarts) = await ShoppingCartService.GetShoppingCartsAsync<E.ShoppingCart>(CreateQueryParamUriShoppingCarts());
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            (result, httpStatusCode, exception, shoppingCartsCount) = await ShoppingCartService.GetShoppingCartsCountAsync<E.ShoppingCart>();
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
        }
        public string CreateQueryParamUriShoppingCarts()
        {
            Dictionary<string, string?> queryParam = [];
            SS.Pagination pagination = new();
            queryParam.Add(nameof(pagination.PageSize), QueryProductCount.ToString());
            queryParam.Add(nameof(pagination.UsePagination), true.ToString());
            queryParam.Add(nameof(pagination.PageIndex), QueryPageIndex.ToString());
            return QueryHelpers.AddQueryString("", queryParam);
        }
        private async Task HandleCountChangeShoppingCartsAsync(int productCount)
        {
            this.QueryPageIndex = 0;
            this.QueryProductCount = productCount;
            (var result, var httpStatusCode, var exception, this.shoppingCarts) = await ShoppingCartService.GetShoppingCartsAsync<E.ShoppingCart>(CreateQueryParamUriShoppingCarts());
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
        }
        private async Task HandlePageIndexChangeShoppingCartsAsync(long pageIndex)
        {
            List<E.ShoppingCart> copyShoppingCarts = this.shoppingCarts != null ? new (this.shoppingCarts) : new ();
            this.QueryPageIndex = (int)pageIndex;
            CancellationTokenSource cancellationTokenSource = new();
            _cancellationTokenPageIndexChange.ForEach(x => x.Cancel());
            _cancellationTokenPageIndexChange.Add(cancellationTokenSource);

            (var result, var httpStatusCode, var exception, this.shoppingCarts) = await ShoppingCartService.GetShoppingCartsAsync<E.ShoppingCart>(CreateQueryParamUriShoppingCarts(), cancellationTokenSource.Token);
            if (!cancellationTokenSource.IsCancellationRequested)
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            else
                this.shoppingCarts = copyShoppingCarts;

            _cancellationTokenPageIndexChange.Remove(cancellationTokenSource);
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
        ~ShoppingCartsList()
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