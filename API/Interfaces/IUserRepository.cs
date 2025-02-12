using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        // Update Methods
        void Update(AppUser user);

        // Save Methods
        Task<bool> SaveAllAsync();

        // Get Methods
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto?> GetMemberAsync(string username);
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<AppUser?> GetUserByUserNameAsync(string username);
        Task<IEnumerable<AppUser>> GetUsersAsync();
    }
}