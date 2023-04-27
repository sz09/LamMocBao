using Shared.Models;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface ISystemSettingService : IService<SystemSetting>
    {
        Task<SystemSetting> LoadAsync();
    }
}
