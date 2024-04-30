using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelephoneCompanyApp.Models;
using TelephoneCompanyApp.ViewModels;

namespace TelephoneCompanyApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
    }

    private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        var propertyName = e.Column.SortMemberPath;
        var direction = e.Column.SortDirection;
        var sortParams = Tuple.Create(propertyName, direction); 
        _viewModel.SortCommand.Execute(sortParams);
    }
}