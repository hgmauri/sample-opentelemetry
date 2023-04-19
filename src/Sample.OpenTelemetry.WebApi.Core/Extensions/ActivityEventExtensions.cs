using OpenTelemetry.Logs;
using OpenTelemetry;
using System.Diagnostics;

namespace Sample.OpenTelemetry.WebApi.Core.Extensions;

public class ActivityEventExtensions : BaseProcessor<LogRecord>
{
	public override void OnEnd(LogRecord data)
	{
		base.OnEnd(data);
		var currentActivity = Activity.Current;
		currentActivity?.AddEvent(new ActivityEvent(data?.State?.ToString() ?? string.Empty));
	}
}