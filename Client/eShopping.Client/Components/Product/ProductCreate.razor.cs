using Catalog.Core.Entities;
using eShopping.Client.Data;
using eShopping.Client.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using E = Catalog.Core.Entities;

namespace eShopping.Client.Components.Product 
{
    public partial class ProductCreate : IDisposable
    {
        [Inject]
        private ICatalogService CatalogService { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private IErrorService ErrorService { get; set; }
        [Parameter, EditorRequired]
        public string PagePathUrl { get; set; }
        private IEnumerable<ProductBrand>? ProductBrands { get; set; }
        private IEnumerable<ProductType>? ProductTypes { get; set; }
        private IEnumerable<ImageFileDirectory>? ProductsImageFolders { get; set; }
        private const string ProductsImageFoldersOptionCreateNew = "-- Create New --";
        private ImageFileDirectory? SelectedImageFolder { get; set; } = new ImageFileDirectory();
        private string NewImageFolder { get; set; } = string.Empty;
        private E.Product? NewProduct { get; set; }
        private IBrowserFile? InputFileSource { get; set; }
        private string? FilePathTarget { get; set; }
        private string? FileNameTarget { get; set; }
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

            (result, httpStatusCode, exception, ProductBrands) = await CatalogService.GetProductBrandsAsync<ProductBrand>(string.Empty);
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            (result, httpStatusCode, exception, ProductTypes) = await CatalogService.GetProductTypesAsync<ProductType>(string.Empty);
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            (result, httpStatusCode, exception, ProductsImageFolders) = await CatalogService.GetProductImageFoldersAsync<ImageFileDirectory>(string.Empty);
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            SelectedImageFolder = ProductsImageFolders?.FirstOrDefault() ?? new ImageFileDirectory();
            NewProduct = new E.Product()
            {
                Brand = new ProductBrand(),
                Type = new ProductType(),
                ImageFileDirectory = new ImageFileDirectory()
            };
        }

        private async Task HandleOnChangeUploadBitmapAsync(InputFileChangeEventArgs inputFileChange)
        {
            if (Environment.GetEnvironmentVariable("PHYSICALFOLDER_LOCATION") is not string storeLocation)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, new ArgumentNullException("PHYSICALFOLDER_LOCATION", "Environment"), null);
                return;
            }
            if (inputFileChange.File.Size > 1 * 1024 * 1024)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, new IOException("File size exceeded 1MB"), null);
                return;
            }
            if (SelectedImageFolder is null)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, new ArgumentNullException(nameof(SelectedImageFolder)), null);
                return;
            }
            if (SelectedImageFolder.Directory.Length > 0 && SelectedImageFolder.Directory.FirstOrDefault() != Path.DirectorySeparatorChar)
                storeLocation = Path.Combine(storeLocation, SelectedImageFolder.Directory);

            InputFileSource = inputFileChange.File;
            FilePathTarget = storeLocation;
            FileNameTarget = inputFileChange.File.Name;

            await Task.CompletedTask;
        }
        private async Task HandleUploadBitmapAsync()
        {
            if (InputFileSource == null)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, new ArgumentNullException(nameof(InputFileSource)), null);
                return;
            }
            if (FilePathTarget == null)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, new ArgumentNullException(nameof(FilePathTarget)), null);
                return;
            }
            if (FileNameTarget == null)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, new ArgumentNullException(nameof(FileNameTarget)), null);
                return;
            }

            try
            {
                using (FileStream fileStream = new(Path.Combine(FilePathTarget, FileNameTarget), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    using (Stream stream = InputFileSource.OpenReadStream(InputFileSource.Size))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, ex, null);
                return;
            }
        }
        private async Task HandleOnClickAddImageLocationAsync()
        {
            if (ProductsImageFolders is null)
            {
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, new ArgumentNullException(nameof(ProductsImageFolders)), null);
                return;
            }
            if (ProductsImageFolders.FirstOrDefault(pif => pif.Directory == this.NewImageFolder) is ImageFileDirectory existingFileDirectory)
                SelectedImageFolder = existingFileDirectory;
            else
            {
                HttpStatusCode? httpStatusCode;
                bool result;
                Exception? exception;

                ImageFileDirectory imageFileDirectory = new() { Directory = this.NewImageFolder, IsDeleted = false };
                (result, httpStatusCode, exception, this.SelectedImageFolder) = await CatalogService.CreateProductImageFoldersAsync<ImageFileDirectory, ImageFileDirectory>(imageFileDirectory);
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
                (result, httpStatusCode, exception, this.ProductsImageFolders) = await CatalogService.GetProductImageFoldersAsync<ImageFileDirectory>(string.Empty);
                await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);
            }
        }
        private async Task Submit(EditContext context)
        {
            if (NewProduct is null)
            {
                ArgumentNullException exceptionNewProduct = new(nameof(NewProduct));
                await ErrorService.AddSmartErrorAsync(true, Navigation.Uri, null, exceptionNewProduct, null);
                return;
            }
            context.IsModified();
            (var result, var httpStatusCode, var exception, var createdProduct) = await CatalogService.CreateProductAsync<E.Product, E.Product>(NewProduct);
            await ErrorService.AddSmartErrorAsync(!result, Navigation.Uri, httpStatusCode, exception, null);

            UriBuilder uriBuilder = new(Navigation.Uri);
            uriBuilder.Query = null;
            uriBuilder.Path = PagePathUrl + "/" + createdProduct!.Id;
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
        ~ProductCreate()
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