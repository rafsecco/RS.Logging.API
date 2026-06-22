namespace RS.Logging.API.Retention;

public class RetentionOptions
{
	public int DefaultDays { get; set; } = 90;
	public double IntervalHours { get; set; } = 24;
	public Dictionary<string, int> PolicyByTenant { get; set; } = new();
}
