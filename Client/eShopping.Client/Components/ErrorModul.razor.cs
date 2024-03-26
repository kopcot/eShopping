using eShopping.Client.Data;
using eShopping.Client.Entities;
using Microsoft.AspNetCore.Components;

namespace eShopping.Client.Components 
{
    public partial class ErrorModul : IDisposable
    {
        [Inject]
        private IErrorService ErrorService { get; set; }
        private long errorsBefore = 0;
        private IEnumerable<ErrorData> Errors = new List<ErrorData>();
        private bool disposedValue;
        private Timer? TimerRefresh {  get; set; }   

        protected override async Task OnInitializedAsync()
        {
            TimerRefresh = new Timer(async _ => await RefreshErrorList(), null, 1000, 1000);

            await base.OnInitializedAsync();
        }
        private async Task HandleAcceptButtonAsync()
        {
            await ErrorService.AcceptAllErrorsAsync();
            this.StateHasChanged();
        }
        private async Task HandleAcceptErrorButtonAsync(ErrorData errorData)
        {
            await ErrorService.AcceptErrorAsync(errorData);
            this.StateHasChanged();
        }
        private async Task RefreshErrorList()
        {
            Errors = await ErrorService.GetAllErrorsAsync();
            if (Errors.Count() != errorsBefore)
            {
                errorsBefore = Errors.Count();
                await InvokeAsync(StateHasChanged);
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
                TimerRefresh?.Dispose(); 
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        ~ErrorModul()
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