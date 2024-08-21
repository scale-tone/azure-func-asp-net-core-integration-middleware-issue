using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication((builderContext, workerAppBuilder) => {

        workerAppBuilder.UseWhen((FunctionContext context) =>
        {
            // This middleware is only for http trigger invocations.
            return context
                .FunctionDefinition
                .InputBindings
                .Values
                .First(a => a.Type.EndsWith("Trigger"))
                .Type == "httpTrigger";
        },
        async (FunctionContext context, Func<Task> next) =>
        {
            // Produces 400 Bad Request
            await next();

            var httpContext = context.GetHttpContext();
            var httpResponse = httpContext!.Response;

            // Prints 200
            Console.WriteLine($"Status Code: {httpResponse.StatusCode}");

            var responseData = context.GetHttpResponseData();

            // Prints null
            Console.WriteLine($"ResponseData: {responseData}");
        });

    })
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
