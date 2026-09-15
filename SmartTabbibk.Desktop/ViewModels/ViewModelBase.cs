using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartTabbibk.Desktop.ViewModels
{
    /// <summary>
    /// الأساس المشترك لكل ViewModel — ده قلب نمط MVVM:
    /// أي تغيير في خاصية هنا بيبلّغ الواجهة (View) تلقائياً تحدّث نفسها،
    /// من غير ما الـ ViewModel يعرف أي حاجة عن الواجهة نفسها (فصل تام بينهم)
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
