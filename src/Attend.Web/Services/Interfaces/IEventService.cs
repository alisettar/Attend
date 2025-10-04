using Attend.Web.Models;

namespace Attend.Web.Services.Interfaces;

public interface IEventService
{
    Task<PaginatedResponse<EventViewModel>> GetEventsAsync(PaginationRequest request);
    Task<EventViewModel?> GetEventByIdAsync(Guid id);
    Task<Guid> CreateEventAsync(EventCreateViewModel model);
    Task<bool> UpdateEventAsync(Guid id, EventUpdateViewModel model);
    Task<bool> DeleteEventAsync(Guid id);
}
