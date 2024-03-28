namespace FishMarket.Api.Extensions;

using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public static class ExceptionHandlerExtensions
{
    private const string LoggerCategoryName = "GlobalExceptionHandler";

    private const string ProblemJsonContentType = "application/problem+json";

    public static void UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseExceptionHandler(builder =>
            builder.Run(async context =>
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature?.Error;

                if (exception != null)
                {
                    // Log the exception
                    var logger = loggerFactory.CreateLogger(LoggerCategoryName);
                    logger.LogError(exception, exception.Message);

                    context.Response.ContentType = ProblemJsonContentType;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var problemDetails = new ProblemDetails
                    {
                        Status = context.Response.StatusCode,
                        Title = "An error occured while processing your request.",
                        Detail = "Please, try again later."
                    };

                    problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context?.TraceIdentifier;

                    var stream = context!.Response.Body;
                    await JsonSerializer.SerializeAsync(stream, problemDetails);
                }
            }));
}

