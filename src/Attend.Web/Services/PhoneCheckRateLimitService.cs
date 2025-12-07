using System.Collections.Concurrent;

namespace Attend.Web.Services;

public interface IPhoneCheckRateLimitService
{
    Task<bool> IsAllowedAsync(string ipAddress, string phone);
}

public class PhoneCheckRateLimitService : IPhoneCheckRateLimitService
{
    // IP bazlı: 10 istek/saat
    private static readonly ConcurrentDictionary<string, List<DateTime>> _ipRequests = new();
    private const int MaxIpRequests = 10;
    private static readonly TimeSpan IpWindow = TimeSpan.FromHours(1);

    // Telefon bazlı: 3 istek/saat
    private static readonly ConcurrentDictionary<string, List<DateTime>> _phoneRequests = new();
    private const int MaxPhoneRequests = 3;
    private static readonly TimeSpan PhoneWindow = TimeSpan.FromHours(1);

    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<bool> IsAllowedAsync(string ipAddress, string phone)
    {
        await _semaphore.WaitAsync();
        try
        {
            var now = DateTime.UtcNow;

            // IP kontrolü
            var ipKey = $"ip:{ipAddress}";
            var ipRequests = _ipRequests.GetOrAdd(ipKey, _ => new List<DateTime>());
            ipRequests.RemoveAll(r => now - r > IpWindow);
            
            if (ipRequests.Count >= MaxIpRequests)
            {
                return false;
            }

            // Telefon kontrolü
            var phoneKey = $"phone:{phone}";
            var phoneRequests = _phoneRequests.GetOrAdd(phoneKey, _ => new List<DateTime>());
            phoneRequests.RemoveAll(r => now - r > PhoneWindow);
            
            if (phoneRequests.Count >= MaxPhoneRequests)
            {
                return false;
            }

            // İzin verildi, kaydet
            ipRequests.Add(now);
            phoneRequests.Add(now);

            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
