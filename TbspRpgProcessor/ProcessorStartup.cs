using Microsoft.Extensions.DependencyInjection;
using TbspRpgProcessor.Processors;

namespace TbspRpgProcessor
{
    public class ProcessorStartup
    {
        public static void InitializeProcessorLayer(IServiceCollection services)
        {
            services.AddScoped<ITbspRpgProcessor, TbspRpgProcessor>();
        }
    }
}