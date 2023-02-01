namespace DeviceAuth.Models;

public class Temperature
{
   public TimeOnly Time { get; set; }

   public int TemperatureC { get; set; }

   public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

