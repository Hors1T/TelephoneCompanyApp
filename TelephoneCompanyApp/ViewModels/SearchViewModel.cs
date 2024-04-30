using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace TelephoneCompanyApp.ViewModels;

public class SearchViewModel : BaseViewModel
{
    private string _searchText;

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public ICommand CloseCommand { get; }

    public SearchViewModel()
    {
        CloseCommand = new RelayCommand(Close);
    }

    private void Close()
    {
        CloseAction?.Invoke();
        SearchCompleted?.Invoke(SearchText);
    }

    public event Action<string> SearchCompleted;
    public Action CloseAction;
}