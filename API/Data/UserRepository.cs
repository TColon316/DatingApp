using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
    {
        public async Task<MemberDto?> GetMemberAsync(string username)
        {
            return await context.Users.Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = context.Users.AsQueryable();

            // Exclude the User from the list of results (So User does not see themselves in the results)
            query = query.Where(x => x.UserName != userParams.CurrentUsername);

            // Filter by Gender filter (If Gender filter is provided)
            if (userParams.Gender != null)
            {
                query = query.Where(x => x.Gender == userParams.Gender);
            }

            // Order the query based on LastActiveDate
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive) // "_" is the the equivalent of the Default Case
            };

            // Get the Min and Max DOB to filter out users outside of Users requested Age range
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            // Apply the filters above to the Query and only return results that satisfy Users requested range
            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            return await PagedList<MemberDto>.CreateAsync
            (
                query.ProjectTo<MemberDto>(mapper.ConfigurationProvider),
                userParams.PageNumber,
                userParams.PageSize
            );
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByUserNameAsync(string username)
        {
            return await context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users.Include(x => x.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            // SaveChanges() returns a value based on the number of changes saved. If greater then 0, means changes were saved
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            // This explicitly tells EF that this entity has been modified.
            context.Entry(user).State = EntityState.Modified;
        }
    }
}