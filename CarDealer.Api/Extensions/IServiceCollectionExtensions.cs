﻿using CarDealer.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CarDealer.Api.Extensions
{
	public static class IServiceCollectionExtensions
	{

		public static IServiceCollection AddQueryOrCommand(this IServiceCollection services, Type handlerInterface)
		{
			var handlers = typeof(ICarDealerContext).Assembly.GetTypes()
				.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
			   );

			foreach (var handler in handlers)
			{
				var item = handler.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface);
				services.AddScoped(item, handler);
			}
			return services;
		}

		public static void AddPollyHttpClient(this IServiceCollection services, ILoggerFactory loggerFactory, string clientName, string baseUrl, int retryCount )
		{
			//random delay between retries
			Random jitter = new Random();
			var logger = loggerFactory.CreateLogger(clientName);

			Action<DelegateResult<HttpResponseMessage>, TimeSpan> onBreak = (exc, time) =>
			 {
				 logger.LogWarning("on Break: Circuit Breaker break retries. Message: , Time delay: {0}", time.TotalMilliseconds);
				 throw new BrokenCircuitException("Service inoperative. Please try again later");
			 };

			services.AddHttpClient(clientName,
				client => client.BaseAddress = new Uri(baseUrl))
					.AddTransientHttpErrorPolicy(builder => builder
						.WaitAndRetryAsync(retryCount,
							retryattempt => TimeSpan.FromSeconds(Math.Pow(2, retryattempt)) + TimeSpan.FromMilliseconds(jitter.Next(0, 100)),
							onRetry: (outcome, timespan, retryAttempt, context) =>
							{
								logger.LogWarning($"Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}.");
							}
							));
					
		}


	}
}
