// See https://aka.ms/new-console-template for more information
using Nager.Date;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        static DateTime isBusinessDay(DateTime dt)
        {
            List<PublicHoliday> ph = GetPublicHolidays();

            if (WeekendSystem.IsWeekend(dt, CountryCode.BR))
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    dt = isBusinessDay(dt.AddDays(-1));
                }
            }
            foreach (var publicHolidays in ph)
            {
                if (publicHolidays.Date == dt)
                    dt = isBusinessDay(dt.AddDays(-1));
            }
            return dt;
        }

        static int GetNumberOfWorkingDays(DateTime start, DateTime stop)
        {
            int days = 0;
            while (start <= stop)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                {
                    ++days;
                }
                start = start.AddDays(1);
            }
            return days;
        }

        static List<PublicHoliday> GetPublicHolidays()
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                int dt = DateTime.Now.Year;
                var response = httpClient.GetAsync($"https://date.nager.at/api/v3/publicholidays/{dt}/BR").GetAwaiter().GetResult();
                List<PublicHoliday> model = JsonSerializer.Deserialize<List<PublicHoliday>>(response.Content.ReadAsStringAsync().Result);
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //DateTime dateTime = DateTime.Now;
        DateTime dateTime = new DateTime(2024,7,9);
        Console.WriteLine("DATA AGORA>>");
        Console.WriteLine(dateTime);
        Console.WriteLine("DATA TODO DIA 25 NO MES SEGUINTE");
        DateTime paymentDate = new DateTime(dateTime.Year, dateTime.Month, 25);
        Console.WriteLine(paymentDate);
        Console.WriteLine("É DIA UTIL??");
        DateTime newPayment = isBusinessDay((DateTime)paymentDate);
        Console.WriteLine(newPayment);
        //var holidays = HolidaySystem.GetHolidays(2024, "BR");
    }
}

class PublicHoliday
{
    public DateTime Date { get; set; }
    public string LocalName { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }
    public bool Fixed { get; set; }
    public bool Global { get; set; }
    public string[] Counties { get; set; }
    public int? LaunchYear { get; set; }
    public string[] Types { get; set; }
}