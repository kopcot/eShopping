using Catalog.Core.Entities;
using Catalog.Core.Specs;
using eShopping.Client.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Shared.Core.Specs;
using System.Net;

namespace eShopping.Client.Pages
{
    public partial class Products : IDisposable
    {
        public const string PagePathUrl = "/products";
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public string PageAction { get; set; }
        public int QuerySorting { get; set; } = (int)ProductSpecParams.SortingType.NoSorting;
        public int QueryProductCount { get; set; } = 10;
        public int QueryPageIndex { get; set; } = 0;
        private PageTab CurrentTab { get; set; }
        private bool DisposedValue { get; set; }

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
            if (Id is not null)
                return PageTab.Single;
            else if (PageAction is null)
                return PageTab.List;
            else if (PageAction == PageActions.CreateNew)
                return PageTab.CreateNew;
            else
                return PageTab.Undefined;
        }

        #region Enums
        internal enum PageTab
        {
            Undefined,
            List,
            Single,
            CreateNew,
            Edit
        }
        internal static class PageActions
        {
            public const string CreateNew = "New";
            public const string Edit = "Edit";
        }
        #endregion Enums

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                DisposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Products()
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