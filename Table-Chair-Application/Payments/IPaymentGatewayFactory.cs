using Microsoft.Extensions.DependencyInjection;
using System;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Payments
{
    public interface IPaymentGatewayFactory
    {
        IPaymentGateway Create(PaymentMethod method);
    }

    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentGatewayFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentGateway Create(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.CreditCard => _serviceProvider.GetRequiredService<StripeGateway>(),
                PaymentMethod.PayPal => _serviceProvider.GetRequiredService<PayPalGateway>(),
                PaymentMethod.Click => _serviceProvider.GetRequiredService<ClickGateway>(),
                PaymentMethod.Payme => _serviceProvider.GetRequiredService<PaymeGateway>(),
                _ => throw new NotSupportedException($"Payment method {method} is not supported")
            };
        }
    }
}

