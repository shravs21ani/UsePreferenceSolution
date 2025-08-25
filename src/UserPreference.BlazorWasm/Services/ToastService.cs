using Microsoft.JSInterop;

namespace UserPreference.BlazorWasm.Services;

public class ToastService : IToastService
{
    private readonly IJSRuntime _jsRuntime;
    public event Action<string, string>? OnToast;

    public ToastService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void ShowSuccess(string message)
    {
        OnToast?.Invoke("success", message);
        ShowToast("success", message);
    }

    public void ShowError(string message)
    {
        OnToast?.Invoke("error", message);
        ShowToast("error", message);
    }

    public void ShowWarning(string message)
    {
        OnToast?.Invoke("warning", message);
        ShowToast("warning", message);
    }

    public void ShowInfo(string message)
    {
        OnToast?.Invoke("info", message);
        ShowToast("info", message);
    }

    private async void ShowToast(string type, string message)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("showToast", type, message);
        }
        catch
        {
            // Fallback to console in case JS is not available
            Console.WriteLine($"[{type.ToUpper()}] {message}");
        }
    }
}
