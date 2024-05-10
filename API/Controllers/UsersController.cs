using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extension;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user =  await _userRepository.GetMembersByUserNameAsync(username);

            return _mapper.Map<MemberDTO>(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdatesDto memberUpdatesDto)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());
            if(user == null) return NotFound();

            _mapper.Map(memberUpdatesDto, user);

            if(await _userRepository.SaveAllAsync()) return NoContent();
            
            return BadRequest("Failed to update.");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user  = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // checking this is the first photo uploaded by user if yes then set as IsMain

            if( user.Photos.Count == 0) photo.IsMain =true;

            user.Photos.Add(photo);

            if( await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser), new{UserName = user.UserName}, _mapper.Map<PhotoDto>(photo));
            };

            return BadRequest("Probliem adding photo.");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> UpdateIsMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            if(user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("Cannot set the photo as main if it's already set as main.");

            var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);

            if(currentMain != null) currentMain.IsMain = false;
            
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return Ok();
            
            return BadRequest("There is some problem, please try again.");

        }

        [HttpDelete("delete-photo/{photoid}")]
        public async Task<ActionResult> DeletePhoto(int photoid)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoid);

            if(photo == null) return NotFound();

            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if(result.Error != null) return BadRequest("There is some problem, please try again.");

            user.Photos.RemoveAll(x=>x.Id == photoid);

            if( await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("There is some problem, please try again.");


        }

    }
}