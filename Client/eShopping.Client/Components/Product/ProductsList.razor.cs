using Catalog.Core.Specs;
using eShopping.Client.Data;
using eShopping.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using OpenTelemetry.Trace;
using System.Net;
using static eShopping.Client.Pages.Products;
using E = Catalog.Core.Entities;
using SS = Shared.Core.Specs;

namespace eShopping.Client.Components.Product
{
    public partial class ProductsList : IDisposable
    {
        [Inject]
        private ICatalogService CatalogService { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private IErrorService ErrorService { get; set; }
        [Inject]
        public Tracer Tracer { get; set; }
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? InputAttributes { get; set; }
        private IEnumerable<E.Product>? products { get; set; }
        [CascadingParameter]
        public long? ProductsCount { get; set; }
        [CascadingParameter(Name = "ProductsPage")]
        public Products ProductsPage { get; set; }
        #region Query parameters
        public ProductSpecParams.SortingType QuerySorting 
        {
            get => ProductsPage.QuerySorting;
            set => ProductsPage.QuerySorting = value;
        }
        public int QueryProductCount
        {
            get => ProductsPage.QueryProductCount;
            set => ProductsPage.QueryProductCount = value;
        }

        public int QueryPageIndex
        {
            get => ProductsPage.QueryPageIndex;
            set => ProductsPage.QueryPageIndex = value;
        }
        [Parameter]
        [SupplyParameterFromQuery(Name = "BrandName")]
        public string? QueryBrandName { get; set; }
        [Parameter]
        [SupplyParameterFromQuery(Name = "TypeName")]
        public string? QueryTypeName { get; set; }
        [Parameter]
        [SupplyParameterFromQuery(Name = "ImageDirectory")]
        public string? QueryImageDirectory { get; set; }
        [Parameter]
        [SupplyParameterFromQuery(Name = "IsDeleted")]
        public string? QueryIsDeleted { get; set; }
        [Parameter]
        [SupplyParameterFromQuery(Name = "PriceOver")]
        public string? QueryPriceOver { get; set; }
        [Parameter]
        [SupplyParameterFromQuery(Name = "PriceUnder")]
        public string? QueryPriceUnder { get; set; }
        #endregion Query parameters
        private readonly List<CancellationTokenSource> _cancellationTokenPageIndexChange = [];
        private ShownAs shownItemsAs { get; set; } = ShownAs.AsItem;
        private int shownItemsAsInt => (int)shownItemsAs;
        private bool disposedValue;

        protected override async Task OnInitializedAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ProductsList)}_{nameof(OnInitializedAsync)}"))
            {
                await base.OnInitializedAsync();
            };
        }
        protected override async Task OnParametersSetAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ProductsList)}_{nameof(OnParametersSetAsync)}"))
            {
                HttpStatusCode? httpStatusCode;
                bool result;
                Exception? exception;

                (result, httpStatusCode, exception, products) = await CatalogService.GetProductByQueryAsync<E.Product>(CreateQueryParamUri());
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                (result, httpStatusCode, exception, ProductsCount) = await CatalogService.GetProductCountAsync<E.Product>(CreateQueryParamUri());
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            };
        }
        public string CreateQueryParamUri()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ProductsList)}_{nameof(CreateQueryParamUri)}"))
            {
                Dictionary<string, string?> queryParam = [];
                SS.Pagination pagination = new();
                ProductSpecParams productSpec = new();
                queryParam.Add(nameof(pagination.PageSize), QueryProductCount.ToString());
                queryParam.Add(nameof(pagination.UsePagination), true.ToString());
                queryParam.Add(nameof(pagination.PageIndex), QueryPageIndex.ToString());
                queryParam.Add(nameof(productSpec.Sorting), QuerySorting.ToString());
                if (QueryBrandName is not null)
                    queryParam.Add(nameof(productSpec.ContainsBrandName), QueryBrandName);
                if (QueryTypeName is not null)
                    queryParam.Add(nameof(productSpec.ContainsTypeName), QueryTypeName);
                if (QueryImageDirectory is not null)
                    queryParam.Add(nameof(productSpec.ContainsImageDirectory), QueryImageDirectory);
                if (QueryPriceOver is not null)
                    queryParam.Add(nameof(productSpec.PriceOver), QueryPriceOver);
                if (QueryPriceUnder is not null)
                    queryParam.Add(nameof(productSpec.PriceUnder), QueryPriceUnder);

                return QueryHelpers.AddQueryString("", queryParam);
            };
        }
        private async Task HandleCountChangeProductsAsync(int productCount)
        {
            this.QueryPageIndex = 0;
            this.QueryProductCount = productCount;
            (var result, var httpStatusCode, var exception, this.products) = await CatalogService.GetProductByQueryAsync<E.Product>(CreateQueryParamUri());
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);

        }
        private async Task HandleSortingChangeAsync(ProductSpecParams.SortingType sorting)
        {
            this.QueryPageIndex = 0;
            this.QuerySorting = sorting;
            (var result, var httpStatusCode, var exception, this.products) = await CatalogService.GetProductByQueryAsync<E.Product>(CreateQueryParamUri());
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
        }
        private async Task HandlePageIndexChangeAsync(long pageIndex)
        {
            List<E.Product> copyProducts = this.products != null ? new(this.products) : new();

            this.QueryPageIndex = (int)pageIndex;
            CancellationTokenSource cancellationTokenSource = new();
            _cancellationTokenPageIndexChange.ForEach(x  => x.Cancel());
            _cancellationTokenPageIndexChange.Add(cancellationTokenSource);

            (var result, var httpStatusCode, var exception, this.products) = await CatalogService.GetProductByQueryAsync<E.Product>(CreateQueryParamUri(), cancellationTokenSource.Token);
            if (!cancellationTokenSource.IsCancellationRequested) 
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            else
                this.products = copyProducts;

            _cancellationTokenPageIndexChange.Remove(cancellationTokenSource);
        }
        public bool ShowProduct { get; set; }
        public int ShowProductId { get; set; }
        private async Task HandleRowClickAsync(int productId)
        {
            UriBuilder uriBuilder = new(Navigation.Uri);
            uriBuilder.Query = null;
            uriBuilder.Path += "/" + productId;
            Navigation.NavigateTo(uriBuilder.Uri.AbsoluteUri);

            await Task.CompletedTask;
        }
        private async Task HandleCreateNewClickAsync()
        {
            UriBuilder uriBuilder = new(Navigation.Uri);
            uriBuilder.Query = null;
            uriBuilder.Path += "/" + PageActions.CreateNew;
            Navigation.NavigateTo(uriBuilder.Uri.AbsoluteUri);
            await Task.CompletedTask;
        }
        private enum ShownAs 
        { 
            AsTable,
            AsItem
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
        ~ProductsList()
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