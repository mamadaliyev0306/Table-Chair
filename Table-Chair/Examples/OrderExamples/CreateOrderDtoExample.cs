using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Examples.OrderExamples
{
    public class CreateOrderDtoExample : IExamplesProvider<CreateOrderDto>
    {
        public CreateOrderDto GetExamples()
        {
            return new CreateOrderDto()
            {
                UserId = 12,
                ShippingAddressId = 5,
                PaymentMethod = PaymentMethod.CreditCard,
                Items = new List<OrderItemCreateDto>
            {
                new OrderItemCreateDto
                {
                    ProductId = 101,
                    Quantity = 2
                },
                new OrderItemCreateDto
                {
                    ProductId = 202,
                    Quantity = 1
                }
            }
            };
        }
    }
}
