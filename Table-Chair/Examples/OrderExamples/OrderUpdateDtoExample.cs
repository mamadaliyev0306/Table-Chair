using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Examples.OrderExamples
{
    public class OrderUpdateDtoExample : IExamplesProvider<OrderUpdateDto>
    {
        public OrderUpdateDto GetExamples()
        {
            return new OrderUpdateDto()
            {
                Id = 1,
                ShippingAddressId = 1,
                PaymentMethod = PaymentMethod.Payme,
                Status = OrderStatus.Cancelled
            };
        }
    }
}
