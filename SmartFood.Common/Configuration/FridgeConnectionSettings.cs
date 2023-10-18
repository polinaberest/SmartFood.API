namespace SmartFood.Common.Configuration;

public class FridgeConnectionSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public decimal NormalTemperature { get; set; } = 4.0M;
    public int OpenCount { get; set; } = 0;
}
