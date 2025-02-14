using Basket.Core.Specs;
using eShopping.Client.Data;
using eShopping.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using OpenTelemetry.Trace;
using System.Net;
using E = Basket.Core.Entities;

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
        [Inject]
        public Tracer Tracer { get; set; }
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? InputAttributes { get; set; }
        [CascadingParameter(Name = "ShoppingCartsPage")]
        public ShoppingCarts ShoppingCartsPage { get; set; }
        [Parameter, EditorRequired]
        public int? ShoppingCartId { get; set; }
        private E.ShoppingCart? shoppingCart { get; set; }
        public ShoppingCartItemSpecParams.SortingType QuerySorting
        {
            get => ShoppingCartsPage.QuerySortingShoppingCartItems;
            set => ShoppingCartsPage.QuerySortingShoppingCartItems = value;
        }
        public long? shoppingCartItemsCount { get; set; }
        private bool disposedValue;

        protected override async Task OnInitializedAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartItemsList)}_{nameof(OnInitializedAsync)}"))
            {
                await base.OnInitializedAsync();
            };
        }
        protected override async Task OnParametersSetAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartItemsList)}_{nameof(OnParametersSetAsync)}"))
            {
                HttpStatusCode? httpStatusCode;
                bool result;
                Exception? exception;

                if (ShoppingCartId is not null)
                {
                    (result, httpStatusCode, exception, shoppingCart) = await ShoppingCartService.GetShoppingCartByIdAsync<E.ShoppingCart>((int)ShoppingCartId, CreateQueryParamUriShoppingCartItems());
                    await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                    (result, httpStatusCode, exception, shoppingCartItemsCount) = await ShoppingCartService.GetShoppingCartItemsCountAsync<E.ShoppingCartItem>((int)ShoppingCartId);
                    await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                }
                else
                    shoppingCart = null;
            };
        }
        public string CreateQueryParamUriShoppingCartItems()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartsList)}_{nameof(CreateQueryParamUriShoppingCartItems)}"))
            {
                Dictionary<string, string?> queryParam = [];
                //SS.Pagination pagination = new();
                ShoppingCartSpecParams shoppingCartSpec = new();
                //queryParam.Add(nameof(pagination.PageSize), QueryProductCount.ToString());
                //queryParam.Add(nameof(pagination.UsePagination), true.ToString());
                //queryParam.Add(nameof(pagination.PageIndex), QueryPageIndex.ToString());
                queryParam.Add(nameof(shoppingCartSpec.Sorting), QuerySorting.ToString());

                return QueryHelpers.AddQueryString("", queryParam);
            };

        }
        private async Task HandleSortingChangeAsync(ShoppingCartItemSpecParams.SortingType sorting)
        {
            //this.QueryPageIndex = 0;
            this.QuerySorting = sorting;
            if (ShoppingCartId is not null)
            {
                (var result, var httpStatusCode, var exception, this.shoppingCart) = await ShoppingCartService.GetShoppingCartByIdAsync<E.ShoppingCart>((int)ShoppingCartId, CreateQueryParamUriShoppingCartItems());
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            }
            else
                this.shoppingCart = null;
        }
        private async Task HandleRowClickShoppingCartsAsync(int productId)
        {
            UriBuilder uriBuilder = new(Navigation.Uri);
            uriBuilder.Query = null;
            uriBuilder.Path += "/" + productId;
            Navigation.NavigateTo(uriBuilder.Uri.AbsoluteUri);
            await Task.CompletedTask;
        }
        private bool deletingInProgress = false;
        private async Task HandleRowClickDeleteShoppingCartsAsync(int productId)
        {
            deletingInProgress = true;
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartItemsList)}_{nameof(HandleRowClickDeleteShoppingCartsAsync)}"))
            {
                (var result, var httpStatusCode, var exception, var deleted) = await ShoppingCartService.DeleteShoppingCartItemByIdAsync<E.ShoppingCartItem>(productId);
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);

                if (ShoppingCartId is not null)
                {
                    (result, httpStatusCode, exception, shoppingCart) = await ShoppingCartService.GetShoppingCartByIdAsync<E.ShoppingCart>((int)ShoppingCartId, CreateQueryParamUriShoppingCartItems());
                    await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                    (result, httpStatusCode, exception, shoppingCartItemsCount) = await ShoppingCartService.GetShoppingCartItemsCountAsync<E.ShoppingCartItem>((int)ShoppingCartId);
                    await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                }
                else
                    shoppingCart = null;
            };
            deletingInProgress = false;
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