using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TelephoneCompanyApp.Models;
using TelephoneCompanyApp.Views;

namespace TelephoneCompanyApp.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly DataAccess _dataAccess;
    private ObservableCollection<AbonentInfo> _abonents;
    private string _filterTextFullName;
    private string _filterTextStreet;
    private string _filterTextHouseNumber;
    private string _filterTextHomePhoneNumber;
    private string _filterTextWorkPhoneNumber;
    private string _filterTextMobilePhoneNumber;

    public string FilterTextFullName
    {
        get { return _filterTextFullName; }
        set
        {
            _filterTextFullName = value;
            OnPropertyChanged(nameof(FilterTextFullName));
            ApplyFilters();
        }
    }

    public string FilterTextStreet
    {
        get { return _filterTextStreet; }
        set
        {
            _filterTextStreet = value;
            OnPropertyChanged(nameof(FilterTextStreet));
            ApplyFilters();
        }
    }

    public string FilterTextHouseNumber
    {
        get { return _filterTextHouseNumber; }
        set
        {
            _filterTextHouseNumber = value;
            OnPropertyChanged(nameof(FilterTextHouseNumber));
            ApplyFilters();
        }
    }

    public string FilterTextHomePhoneNumber
    {
        get { return _filterTextHomePhoneNumber; }
        set
        {
            _filterTextHomePhoneNumber = value;
            OnPropertyChanged(nameof(FilterTextHomePhoneNumber));
            ApplyFilters();
        }
    }

    public string FilterTextWorkPhoneNumber
    {
        get { return _filterTextWorkPhoneNumber; }
        set
        {
            _filterTextWorkPhoneNumber = value;
            OnPropertyChanged(nameof(FilterTextWorkPhoneNumber));
            ApplyFilters();
        }
    }

    public string FilterTextMobilePhoneNumber
    {
        get { return _filterTextMobilePhoneNumber; }
        set
        {
            _filterTextMobilePhoneNumber = value;
            OnPropertyChanged(nameof(FilterTextMobilePhoneNumber));
            ApplyFilters();
        }
    }

    public ObservableCollection<AbonentInfo> Abonents
    {
        get => _abonents;
        set
        {
            _abonents = value;
            OnPropertyChanged(nameof(Abonents));
        }
    }

    public ICommand OpenSearchCommand { get; }
    public ICommand ExportToCSVCommand { get; }
    public ICommand OpenStreetsCommand { get; }
    public ICommand SortCommand { get; }

    public MainViewModel()
    {
        OpenSearchCommand = new RelayCommand(OpenSearchWindow);
        ExportToCSVCommand = new RelayCommand(ExportToCSV);
        OpenStreetsCommand = new RelayCommand(OpenStreetsWindow);
        SortCommand = new RelayCommand<Tuple<string, ListSortDirection?>>(Sort);
        _dataAccess = new DataAccess("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;");
        Abonents = new ObservableCollection<AbonentInfo>(_dataAccess.GetAbonentsInfo());
    }

    private void ApplyFilters()
    {
        Abonents = new ObservableCollection<AbonentInfo>(_dataAccess.GetAbonentsInfo());
        var view = CollectionViewSource.GetDefaultView(Abonents);
        view.Filter = (obj) =>
        {
            if (obj is not AbonentInfo abonent)
                return false;
            var fullNameMatch = string.IsNullOrEmpty(FilterTextFullName) ||
                                abonent.FullName.Contains(FilterTextFullName);
            var streetMatch = string.IsNullOrEmpty(FilterTextStreet) || abonent.Street.Contains(FilterTextStreet);
            var houseNumberMatch = string.IsNullOrEmpty(FilterTextHouseNumber) ||
                                   abonent.HouseNumber.Contains(FilterTextHouseNumber);
            var homePhoneNumberMatch = string.IsNullOrEmpty(FilterTextHomePhoneNumber) ||
                                       abonent.HomePhoneNumber.Contains(FilterTextHomePhoneNumber);
            var workPhoneNumberMatch = string.IsNullOrEmpty(FilterTextWorkPhoneNumber) ||
                                       abonent.WorkPhoneNumber.Contains(FilterTextWorkPhoneNumber);
            var mobilePhoneNumberMatch = string.IsNullOrEmpty(FilterTextMobilePhoneNumber) ||
                                         abonent.MobilePhoneNumber.Contains(FilterTextMobilePhoneNumber);

            return fullNameMatch && streetMatch && houseNumberMatch && homePhoneNumberMatch && workPhoneNumberMatch &&
                   mobilePhoneNumberMatch;
        };
        Abonents = new ObservableCollection<AbonentInfo>(view.Cast<AbonentInfo>());
        view.Refresh();
    }

    private void Sort(Tuple<string, ListSortDirection?> sortParams)
    {
        var propertyName = sortParams.Item1;
        var direction = ListSortDirection.Ascending;
        switch (sortParams.Item2)
        {
            case null:
            case ListSortDirection.Descending:
                direction = ListSortDirection.Ascending;
                break;
            case ListSortDirection.Ascending:
                direction = ListSortDirection.Descending;
                break;
        }

        var view = CollectionViewSource.GetDefaultView(Abonents);

        view.SortDescriptions.Clear();

        if (!string.IsNullOrEmpty(propertyName))
        {
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }
    }

    private void OpenSearchWindow()
    {
        var searchViewModel = new SearchViewModel();
        searchViewModel.SearchCompleted += SearchAbonentsByPhoneNumber;
        var searchWindow = new SearchWindow { DataContext = searchViewModel };
        searchViewModel.CloseAction = () => searchWindow.Close();
        searchWindow.ShowDialog();
    }

    private void SearchAbonentsByPhoneNumber(string phoneNumber)
    {
        var searchResult = _dataAccess.SearchAbonentsByPhoneNumber(phoneNumber);

        if (searchResult.Count == 0)
        {
            MessageBox.Show($"Абоненты с номером телефона \"{phoneNumber}\" не найдены.");
            return;
        }

        Abonents = new ObservableCollection<AbonentInfo>(searchResult);
    }

    private void OpenStreetsWindow()
    {
        var streetsViewModel = new StreetsViewModel();
        var streetsWindow = new StreetsWindow { DataContext = streetsViewModel };
        streetsWindow.ShowDialog();
    }

    private void ExportToCSV()
    {
        var view = CollectionViewSource.GetDefaultView(Abonents);
        Abonents = new ObservableCollection<AbonentInfo>(view.Cast<AbonentInfo>());
        _dataAccess.ExportToCSV(Abonents.ToList());
    }
}