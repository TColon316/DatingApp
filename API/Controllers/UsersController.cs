using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IMapper mapper, IPhotoService photoService, IUserRepository userRepository) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")] // /api/users/3
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // Attempt to retrieve requested User
            var user = await userRepository.GetMemberAsync(username);

            // If User is not found in the database, then return a NotFound exception
            if (user == null)
            {
                return NotFound("User was not found");
            }

            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // Pass username parameter to see if there is a user in the system
            var user = await userRepository.GetUserByUserNameAsync(User.GetUsername());

            // If no User is found, then return an exception indicating this
            if (user == null)
                return BadRequest("Could not find user");

            // This will transfer the changes made by the user (Parameter memberUpdateDto) to the user record pulled from the DB
            mapper.Map(memberUpdateDto, user);

            // Save all changes
            if (await userRepository.SaveAllAsync())
                return NoContent(); // This is the correct response for a successful HttpPut request

            // If save is unsuccessful then return a BadRequest
            return BadRequest("Failed to update the user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // Get the UserName
            var user = await userRepository.GetUserByUserNameAsync(User.GetUsername());

            // End flow if no User was found
            if (user == null)
                return BadRequest("Cannot update user");

            // Send Photo to be added to Cloudinary
            var result = await photoService.AddPhotoAsync(file);

            // Check if there was any issue with adding the Photo
            if (result.Error != null)
                return BadRequest(result.Error.Message);

            // Create a new Photo object to store the Public ID and the Uri where we can access the photo
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // Add the new Photo to the Photo table
            user.Photos.Add(photo);

            // Save changes to the database
            if (await userRepository.SaveAllAsync())
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
            // return mapper.Map<PhotoDto>(photo);

            // If code reaches this point, some other error has occurred
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            // Find User
            var user = await userRepository.GetUserByUserNameAsync(User.GetUsername());

            // Check if User was found
            if (user == null) return BadRequest("Could not find user");

            // Find Photo using the PhotoId that was provided
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            // Check if a Photo was found or that Photo is NOT already the main photo
            if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

            // Get Users current Main Photo
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            // If User already has a current Main Photo, then set the current Main Photo to false
            if (currentMain != null) currentMain.IsMain = false;

            // Set the new Photo to the Main Photo
            photo.IsMain = true;

            // Save Changes to the database and return a Status 204 NoContent response (The appropriate Response for a successful Put request )
            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("There was an issue setting the User's main photo");
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetUserByUserNameAsync(User.GetUsername());

            // This line of code REALLY should never be encounterd. It's mainly just to stop the compiler from complaining about User being nullable
            if (user == null) return BadRequest("User not found");

            // Get the Photo the User is trying to delete
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            // Check if the Photo was found OR if the User is trying to delete their main photo
            if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");

            // Check if Photo was found, then attempt to delete the photo and return the error message if there was any issues
            if (photo.PublicId != null)
            {
                var results = await photoService.DeletePhotoAsync(photo.PublicId);

                if (results.Error != null) return BadRequest(results.Error.Message);
            }

            // Remove the Photo from the User's list of Photos
            user.Photos.Remove(photo);

            // Commit the Delete Request to the database
            if (await userRepository.SaveAllAsync()) return Ok();

            // Return an error message if anything went wrong
            return BadRequest("Problem deleting the Photo");
        }
    }
}