namespace DeviceAuth.Models;

public record DeviceLogin
{
   public required string SerialNumber { get; init; }
   public required string Secret { get; init; }
}
