using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Entities.Commands;

namespace FrontApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            services.AddControllers();
            services.AddEndpointsApiExplorer()
                .AddSwaggerGen();

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, rabbitConfig) =>
                {
                    rabbitConfig.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    rabbitConfig.ConfigureEndpoints(context);
                });

                EndpointConvention.Map<CreatePermitRequest>(new Uri($"queue:{nameof(CreatePermitRequest)}"));
            });

            var app = builder.Build();

            app.UseSwagger()
                .UseSwaggerUI()
                .UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}