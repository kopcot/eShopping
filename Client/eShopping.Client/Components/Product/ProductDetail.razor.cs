using eShopping.Client.Data;
using eShopping.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using E = Catalog.Core.Entities;

namespace eShopping.Client.Components.Product
{
    public partial class ProductDetail : IDisposable
    {
        [Inject]
        private ICatalogService CatalogService { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private IErrorService ErrorService { get; set; }
        [Parameter, EditorRequired]
        public int? ProductId { get; set; }

        private E.Product? Product { get; set; }
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

            if (ProductId is not null)
            {
                (result, httpStatusCode, exception, Product) = await CatalogService.GetProductByIdAsync<E.Product>((int)ProductId);
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            }
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
        ~ProductDetail()
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