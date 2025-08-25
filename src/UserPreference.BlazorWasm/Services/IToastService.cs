namespace UserPreference.BlazorWasm.Services;

public interface IToastService
{
    event Action<string, string> OnToast;
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowWarning(string message);
    void ShowInfo(string message);
}
