using Microsoft.AspNetCore.Components;

namespace eShopping.Common.Components
{
    public partial class RightPanel : IDisposable
    {

        [Parameter]
        public string ModalCssStyle { get; set; }
        [Parameter]
        public RenderFragment HeadContent { get; set; }
        [Parameter]
        public string HeadContentCssStyle { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public string ChildContentCssStyle { get; set; }
        [Parameter]
        public RenderFragment FooterContent { get; set; }
        [Parameter]
        public string FooterContentCssStyle { get; set; }
        [Parameter]
        public string Title { get; set; }
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? InputAttributes { get; set; }

        [Parameter, EditorRequired]
        public bool Visible { get; set; }
        private string VisiblePanel => Visible ? "panel-visible" : "panel-hidden";
        private string VisiblePanelContent => Visible ? "panel-content-visible" : "panel-content-hidden";

        [Parameter]
        public EventCallback<bool> OnClose { get; set; }
        private async Task OnCloseAction()
        {
            Visible = false;
            if (OnClose.HasDelegate)
                await OnClose.InvokeAsync(this.Visible);
        }

        #region Dispose
        private bool disposedValue;
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
        // ~Panel()
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
        #endregion
    }
}