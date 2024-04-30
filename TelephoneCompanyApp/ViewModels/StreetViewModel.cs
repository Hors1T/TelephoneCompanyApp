using TelephoneCompanyApp.Models;

namespace TelephoneCompanyApp.ViewModels;
using System.Collections.ObjectModel;

public class StreetsViewModel : BaseViewModel
{
    private readonly DataAccess _dataAccess;
    private ObservableCollection<StreetInfo> _streets;

    public ObservableCollection<StreetInfo> Streets
    {
        get => _streets;
        set
        {
            _streets = value;
            OnPropertyChanged();
        }
    }

    public StreetsViewModel()
    {
        _dataAccess = new DataAccess("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;");
        Streets = new ObservableCollection<StreetInfo>(_dataAccess.GetStreetInfo());
    }
}