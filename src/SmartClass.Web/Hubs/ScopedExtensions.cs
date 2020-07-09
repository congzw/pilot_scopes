using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Common.DependencyInjection;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;

// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public static class ScopedExtensions
    {
        public static IServiceCollection AddClientMonitors(this IServiceCollection services)
        {
            //todo: 不用第三方的DI工具，如何支持Decorator?
            services.AddTransient<ISignalREventDispatcher, SignalREventDispatcher>();
            services.AddScoped<SignalREventBus>();
            //services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped, typeof(SignalREventHandlerDecorator));
            services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped);

            services.AddScoped<ClientInvokeProcessBus>();
            services.AddAllImpl<IClientInvokeProcess>(ServiceLifetime.Scoped);
            
            services.AddScoped<ClientStubProcessBus>();
            services.AddAllImpl<IClientStubProcess>(ServiceLifetime.Scoped);

            //todo by config
            //var traceEventBus = false;
            //if (traceEventBus)
            //{
            //    services.Decorate<ISignalREventHandler, SignalREventHandlerDecorator>();
            //}


            services.AddSingleton<IEventLogHelper, HubClientLogHelper>();
            services.AddSingleton<HubCallerContextCache>();
            services.AddSingleton<IClientConnectionRepository, ClientConnectionRepository>();
            services.AddScoped<IClientMonitor, ClientMonitor>();
            return services;
        }

        public static IApplicationBuilder UseClientMonitors(this IApplicationBuilder sp)
        {
            EventLogHelper.Resolve = () => sp.ApplicationServices.GetRequiredService<IEventLogHelper>();
            return sp;
        }

        public static class QueryHandlerRegistration
        {
            //services.AddDecorator<IEmailMessageSender, EmailMessageSenderWithRetryDecorator>(decorateeServices =>
            //{
            //    decorateeServices.AddScoped<IEmailMessageSender, SmtpEmailMessageSender>();
            //});

            //public static IServiceCollection RegisterQueryHandler<TQueryHandler, TQuery, TResult>(this IServiceCollection services)
            //    where TQuery : IQuery<TResult>
            //    where TQueryHandler : class, IQueryHandler<TQuery, TResult>
            //{
            //    services.AddTransient<TQueryHandler>();
            //    services.AddTransient<IQueryHandler<TQuery, TResult>>(x =>
            //        new LoggingDecorator<TQuery, TResult>(x.GetService<ILogger<TQuery>>(), x.GetService<TQueryHandler>()));
            //    return services;
            //}
        }
    }

    ////todo: di Decorator with handler
    //public class SignalREventHandlerDecorator : ISignalREventHandler
    //{
    //    private readonly ISignalREventHandler _signalREventHandler;

    //    public SignalREventHandlerDecorator(ISignalREventHandler signalREventHandler)
    //    {
    //        _signalREventHandler = signalREventHandler;
    //    }

    //    public float HandleOrder
    //    {
    //        get => _signalREventHandler.HandleOrder;
    //        set => _signalREventHandler.HandleOrder = value;
    //    }

    //    public bool ShouldHandle(ISignalREvent @event)
    //    {
    //        return _signalREventHandler.ShouldHandle(@event);
    //    }

    //    public async Task HandleAsync(ISignalREvent @event)
    //    {
    //        var theEvent = (SignalREvent)@event;
    //        var hubClients = theEvent.TryGetHubClients();
    //        var eventName = theEvent.GetType().Name;
    //        Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", eventName, JsonConvert.SerializeObject(theEvent.Bags, Formatting.None)));
    //        var info = new MonitorInvokeInfo();
    //        info.ScopeId = theEvent.ScopeId;
    //        info.ClientId = ""; //todo: read from context claims
    //        info.Desc = eventName;
    //        //todo with a api
    //        ManageMonitorHelper.Instance.Config.UpdateMonitorInfoEnabled = true;
    //        await ManageMonitorHelper.Instance.UpdateMonitorInfo(hubClients, info);

    //        await _signalREventHandler.HandleAsync(@event).ConfigureAwait(false);
    //    }
    //}
}
