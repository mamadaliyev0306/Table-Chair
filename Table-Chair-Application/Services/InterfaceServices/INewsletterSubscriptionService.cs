using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface INewsletterSubscriptionService
    {
        Task<IEnumerable<NewsletterSubscriptionDto>> GetAllAsync();
        Task<NewsletterSubscriptionDto> GetByIdAsync(int id);
        Task AddNewsletterSubscription(NewsletterSubscriptionCreateDto newsletterSubscriptionDto);
        Task UpdateAsync(NewsletterSubscriptionDto newsletterSubscriptionDto);
        Task DeleteNewsletterSubscriptionAsync(int id);
        Task SoftDeleteNewsletterSubscriptionAsync(int id);
        Task RestoreNewsletterSubscriptionAsync(int id);
    }
}
