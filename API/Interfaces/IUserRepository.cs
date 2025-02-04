using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        // Update Methods
        void Update(AppUser user);

        // Save Methods
        Task<bool> SaveAllAsync();

        // Get Methods
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto?> GetMemberAsync(string username);
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<AppUser?> GetUserByNameAsync(string username);
        Task<IEnumerable<AppUser>> GetUsersAsync();
    }
}