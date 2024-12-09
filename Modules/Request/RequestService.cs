using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Modules.Security;
using RequestModule;
using ResourceManager;

namespace RequestModule
{
    public class RequestService : IRequestService
    {
        private readonly List<Request> _requests = new List<Request>()
        {
    
            new Request
            {
                Id = 1,
                Title = "Fixed Request 1",
                Description = "This is a fixed request added manually.",
                Status = RequestStatus.New,
                CreatedAt = DateTime.Now,
                User = new User("email1@example.com", "John Doe", "123456789", "123 Main St", true),
                Address = new Address("City1", "Street1", "123", "Apt 1", "12345"),
                ResourcesRequired = new List<Resource>
                {
                    new Resource("pomidor", Category.Food, 3),
                    new Resource("chleb", Category.Food, 5),
                },
                Categories = new List<string> { "Category1" },
                IsVerified = true,
                HandlingOrganization = "Org1"
            },
            new Request
            {
                Id = 2,
                Title = "Fixed Request 2",
                Description = "This is a fixed request added manually.",
                Status = RequestStatus.New,
                CreatedAt = DateTime.Now,
                User = new User("email2@example.com", "Jane Doe", "987654321", "456 Main St", true),
                Address = new Address("City2", "Street2", "456", "Apt 2", "67890"),
                ResourcesRequired = new List<Resource>
                {
                    new Resource("ogórek", Category.Food, 20),
                    new Resource("ser", Category.Food, 500),
                },
                Categories = new List<string> { "Category2" },
                IsVerified = true,
                HandlingOrganization = "Org2"
            }
        };

        public Task<List<Request>> GetRequestsAsync()
        {
            return Task.FromResult(_requests);
        }

        public Task<Request> GetRequestByIdAsync(int id)
        {
            var request = _requests.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(request);
        }

        public Task AddRequestAsync(Request request)
        {
            request.Id = _requests.Count + 1;
            request.CreatedAt = DateTime.Now;
            _requests.Add(request);
            return Task.CompletedTask;
        }

        public Task UpdateRequestAsync(Request request)
        {
            var existingRequest = _requests.FirstOrDefault(r => r.Id == request.Id);
            if (existingRequest != null)
            {
                existingRequest.Title = request.Title;
                existingRequest.Description = request.Description;
                existingRequest.Status = request.Status;
                existingRequest.DateUpdated = DateTime.Now;
                existingRequest.User = request.User;
                existingRequest.Address = request.Address;
                existingRequest.ResourcesRequired = request.ResourcesRequired;
                existingRequest.IsVerified = request.IsVerified;
                existingRequest.HandlingOrganization = request.HandlingOrganization;
                existingRequest.Categories = request.Categories;
            }
            return Task.CompletedTask;
        }

        public Task DeleteRequestAsync(int id)
        {
            var request = _requests.FirstOrDefault(r => r.Id == id);
            if (request != null)
            {
                _requests.Remove(request);
            }
            return Task.CompletedTask;
        }
    }
}