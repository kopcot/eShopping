using Basket.Core.Entities;
using eShopping.Client.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Shared.Core.Specs;
using System.Net;

namespace eShopping.Client.Pages
{
    public partial class ShoppingCarts : IDisposable
    {

        public const string PagePathUrl = "/baskets";
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public int? Id2 { get; set; }
        [Parameter]
        public string? PageAction { get; set; }
        #region Query parameters
        [CascadingParameter]
        public int QueryProductCount { get; set; } = 10;
        [CascadingParameter]
        public int QueryPageIndex { get; set; } = 0;
        #endregion Query parameters
        private PageTab CurrentTab;
        private bool disposedValue;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            CurrentTab = GetPageTab();

            await Task.CompletedTask;
        }
        private PageTab GetPageTab()
        {
            if (Id is not null && Id2 is not null)
                return PageTab.SingleItem;
            else if (Id is not null)
                return PageTab.Single;
            else if (PageAction is null)
                return PageTab.List;
            else 
                return PageTab.Undefined;
        }

        #region Enums
        internal enum PageTab
        {
            Undefined,
            List,
            Single,
            SingleItem
        }

        #endregion Enums
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

         ~ShoppingCarts()
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