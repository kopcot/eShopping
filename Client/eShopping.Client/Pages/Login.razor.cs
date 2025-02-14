using eShopping.Client.Data;
using Microsoft.AspNetCore.Components;
using OpenTelemetry.Trace;

namespace eShopping.Client.Pages
{
    public partial class Login
    {
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public Tracer Tracer { get; set; }
        private bool showPassword { get; set; }
        private bool showPanel { get; set; }
        private bool showModal { get; set; }
        private string passwordType => showPassword ? "text" : "password";
        public const string PagePathUrl = "/login";

        protected override async Task OnInitializedAsync()
        {
            using var span = Tracer.StartActiveSpan($"{nameof(Login)}_{nameof(OnInitializedAsync)}");
            await base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            using var span = Tracer.StartActiveSpan($"{nameof(Login)}_{nameof(OnParametersSetAsync)}");
            await Task.CompletedTask;
        }
    }
}