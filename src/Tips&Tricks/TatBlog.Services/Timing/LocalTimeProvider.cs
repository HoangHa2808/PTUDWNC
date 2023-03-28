namespace TatBlog.Services.Timing;

public class LocalTimeProvider : ITimeProvider
{
	public DateTime Now => DateTime.Now;

	public DateTime Today => DateTime.Now.Date;
}