using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Windows;
using TelephoneCompanyApp.Models;

namespace TelephoneCompanyApp;

public class DataAccess
{
    private readonly string _connectionString;

    public DataAccess(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<AbonentInfo> GetAbonentsInfo()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            var query = "SELECT " +
                        "Ab.FullName AS FullName, " +
                        "Addr.Street AS Street, " +
                        "Addr.HouseNumber AS HouseNumber, " +
                        "HomePhone.Number AS HomePhoneNumber, " +
                        "WorkPhone.Number AS WorkPhoneNumber, " +
                        "MobilePhone.Number AS MobilePhoneNumber " +
                        "FROM Abonents AS Ab " +
                        "JOIN Addresses AS Addr ON Ab.AddressId = Addr.Id " +
                        "JOIN PhoneNumbers AS HomePhone ON Ab.HomePhoneNumberId = HomePhone.Id " +
                        "JOIN PhoneNumbers AS WorkPhone ON Ab.WorkPhoneNumberId = WorkPhone.Id " +
                        "JOIN PhoneNumbers AS MobilePhone ON Ab.MobilePhoneNumberId = MobilePhone.Id;";
            return db.Query<AbonentInfo>(query).ToList();
        }
    }
    
    public List<AbonentInfo> SearchAbonentsByPhoneNumber(string phoneNumber)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            var query = "SELECT " +
                        "Ab.Id AS AbonentId, " +
                        "Ab.FullName AS FullName, " +
                        "Addr.Street AS Street, " +
                        "Addr.HouseNumber AS HouseNumber, " +
                        "HomePhone.Number AS HomePhoneNumber, " +
                        "WorkPhone.Number AS WorkPhoneNumber, " +
                        "MobilePhone.Number AS MobilePhoneNumber " +
                        "FROM Abonents AS Ab " +
                        "JOIN Addresses AS Addr ON Ab.AddressId = Addr.Id " +
                        "JOIN PhoneNumbers AS HomePhone ON Ab.HomePhoneNumberId = HomePhone.Id " +
                        "JOIN PhoneNumbers AS WorkPhone ON Ab.WorkPhoneNumberId = WorkPhone.Id " +
                        "JOIN PhoneNumbers AS MobilePhone ON Ab.MobilePhoneNumberId = MobilePhone.Id " +
                        "WHERE " +
                        "HomePhone.Number = @PhoneNumber OR " +
                        "WorkPhone.Number = @PhoneNumber OR " +
                        "MobilePhone.Number = @PhoneNumber;";
            return db.Query<AbonentInfo>(query, new { PhoneNumber = phoneNumber }).ToList();
        }
    }

    public List<StreetInfo> GetStreetInfo()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            var query = "SELECT Str.Name AS Name, COUNT(DISTINCT Ab.Id) AS NumberOfSubscribers" +
                        "\nFROM Abonents AS Ab" +
                        "\nJOIN Addresses AS Addr ON Ab.AddressId = Addr.Id" +
                        "\nJOIN Streets AS Str ON Addr.Street = Str.Name"+
                        "\nGROUP BY Str.Name;";
            return db.Query<StreetInfo>(query).ToList();
        }
        
    }
    public void ExportToCSV(List<AbonentInfo> abonents)
    {
        // Здесь вы можете указать путь к файлу CSV
        var filePath = $"report_{DateTime.Now.ToString("{MM.dd.yyyy HH.mm}", CultureInfo.InvariantCulture)}.csv";

        try
        {
            // Создаем новый экземпляр StreamWriter для записи в файл CSV
            using (var writer = new StreamWriter(filePath))
            {
                // Записываем заголовки столбцов
                writer.WriteLine("ФИО абонента, Улица, Номер дома, Номер телефона (домашний), Номер телефона (рабочий), Номер телефона (мобильный)");

                // Проходимся по каждому абоненту в отсортированных данных и записываем его в файл CSV
                foreach (var abonent in abonents)
                {
                    // Формируем строку данных для текущего абонента
                    var line = $"{abonent.FullName}, {abonent.Street}, {abonent.HouseNumber}, {abonent.HomePhoneNumber}, {abonent.WorkPhoneNumber}, {abonent.MobilePhoneNumber}";
                    // Записываем строку в файл
                    writer.WriteLine(line);
                }
            }

            MessageBox.Show("Данные успешно экспортированы в файл CSV.", "Экспорт в CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при экспорте данных в CSV: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}