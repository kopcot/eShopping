using eShopping.Client.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenTelemetry.Trace;
using E = Catalog.Core.Entities;

namespace eShopping.Client.Components.Product
{
    public partial class ProductTypeCreate : IDisposable
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
        private E.ProductType ProductType { get; set; } = new E.ProductType();
        private E.ProductType? CreatedProductType { get; set; }
        private bool disposedValue;
        protected override async Task OnInitializedAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ProductTypeCreate)}_{nameof(OnInitializedAsync)}"))
            {
                await base.OnInitializedAsync();
            };
        }
        protected override async Task OnParametersSetAsync()
        {
            using (var span = Tracer.StartActiveSpan($"{nameof(ProductTypeCreate)}_{nameof(OnParametersSetAsync)}"))
            {
                await base.OnParametersSetAsync();
            };
        }
        private async Task Submit(EditContext context)
        {
            if (ProductType is null)
            {
                ArgumentNullException exceptionNewProduct = new(nameof(ProductType));
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, exceptionNewProduct, null);
                return;
            }
            context.IsModified();
            (var result, var httpStatusCode, var exception, CreatedProductType) = await CatalogService.CreateProductTypeAsync<E.ProductType, E.ProductType>(ProductType);
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BrandCreate()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose
    }
}