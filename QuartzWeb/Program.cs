using Quartz;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddRazorPages();

        _ = builder.Services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(ExchangeTradeSaveJob));
            _ = q.AddJob<ExchangeTradeSaveJob>(opts => opts.WithIdentity(jobKey));
            _ = q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ExchangeTradeSaveJob-trigger")
                .WithSimpleSchedule(b => b.WithIntervalInSeconds(2).WithMisfireHandlingInstructionNextWithRemainingCount().RepeatForever())
                .StartAt(DateTimeOffset.Now.AddSeconds(10))
            );
        });

        _ = builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        var app = builder.Build();

        app.UseRouting();

        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        app.Run();
    }
}
