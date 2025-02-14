using Basket.Core.Specs;
using eShopping.Client.Data;
using eShopping.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using OpenTelemetry.Trace;
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
        [Inject]
        public Tracer Tracer { get; set; }
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? InputAttributes { get; set; }
        [CascadingParameter(Name = "ShoppingCartsPage")]
        public ShoppingCarts ShoppingCartsPage { get; set; }
        #region Query parameters
        public ShoppingCartSpecParams.SortingType QuerySorting
        {
            get => ShoppingCartsPage.QuerySortingShoppingCarts;
            set => ShoppingCartsPage.QuerySortingShoppingCarts = value;
        }
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
        #endregion Query parameters
        private IEnumerable<E.ShoppingCart>? shoppingCarts;
        public long? shoppingCartsCount { get; set; }
        private readonly List<CancellationTokenSource> _cancellationTokenPageIndexChange = [];
        private bool deletingInProgress = false;
        private bool sortingInProgress = false;
        private bool disposedValue;

        protected override async Task OnInitializedAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartsList)}_{nameof(OnInitializedAsync)}"))
            { 
                await base.OnInitializedAsync();
            };
        }
        protected override async Task OnParametersSetAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartsList)}_{nameof(OnParametersSetAsync)}"))
            { 
                HttpStatusCode? httpStatusCode;
                bool result;
                Exception? exception;

                (result, httpStatusCode, exception, shoppingCarts) = await ShoppingCartService.GetShoppingCartsAsync<E.ShoppingCart>(CreateQueryParamUriShoppingCarts());
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                (result, httpStatusCode, exception, shoppingCartsCount) = await ShoppingCartService.GetShoppingCartsCountAsync<E.ShoppingCart>();
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            };
        }
        public string CreateQueryParamUriShoppingCarts()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartsList)}_{nameof(CreateQueryParamUriShoppingCarts)}"))
            {
                Dictionary<string, string?> queryParam = [];
                SS.Pagination pagination = new();
                ShoppingCartSpecParams shoppingCartSpec = new();
                queryParam.Add(nameof(pagination.PageSize), QueryProductCount.ToString());
                queryParam.Add(nameof(pagination.UsePagination), true.ToString());
                queryParam.Add(nameof(pagination.PageIndex), QueryPageIndex.ToString());
                queryParam.Add(nameof(shoppingCartSpec.Sorting), QuerySorting.ToString());

                return QueryHelpers.AddQueryString("", queryParam);
            };

        }
        private async Task HandleCountChangeShoppingCartsAsync(int productCount)
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartsList)}_{nameof(HandleCountChangeShoppingCartsAsync)}"))
            {
                this.QueryPageIndex = 0;
                this.QueryProductCount = productCount;
                (var result, var httpStatusCode, var exception, this.shoppingCarts) = await ShoppingCartService.GetShoppingCartsAsync<E.ShoppingCart>(CreateQueryParamUriShoppingCarts());
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            };
        }
        private async Task HandleSortingChangeAsync(ShoppingCartSpecParams.SortingType sorting)
        {
            this.QueryPageIndex = 0;
            this.QuerySorting = sorting;
            (var result, var httpStatusCode, var exception, this.shoppingCarts) = await ShoppingCartService.GetShoppingCartsAsync<E.ShoppingCart>(CreateQueryParamUriShoppingCarts());
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
        }
        private async Task HandlePageIndexChangeShoppingCartsAsync(long pageIndex)
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartsList)}_{nameof(HandlePageIndexChangeShoppingCartsAsync)}"))
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
            };
        }
        private async Task HandleRowClickShoppingCartsAsync(int productId)
        {
            UriBuilder uriBuilder = new(Navigation.Uri);
            uriBuilder.Query = null;
            uriBuilder.Path += "/" + productId;
            Navigation.NavigateTo(uriBuilder.Uri.AbsoluteUri);
            await Task.CompletedTask;
        }
        private async Task HandleRowClickDeleteShoppingCartsAsync(int productId)
        {
            deletingInProgress = true;
            using (var span = Tracer.StartActiveSpan($"{nameof(ShoppingCartsList)}_{nameof(HandleRowClickDeleteShoppingCartsAsync)}"))
            {
                (var result, var httpStatusCode, var exception, var deleted) = await ShoppingCartService.DeleteShoppingCartByIdAsync<E.ShoppingCart>(productId);
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);

                (result, httpStatusCode, exception, shoppingCarts) = await ShoppingCartService.GetShoppingCartsAsync<E.ShoppingCart>(CreateQueryParamUriShoppingCarts());
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                (result, httpStatusCode, exception, shoppingCartsCount) = await ShoppingCartService.GetShoppingCartsCountAsync<E.ShoppingCart>();
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
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
                _cancellationTokenPageIndexChange.ForEach(x => x.Cancel());
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