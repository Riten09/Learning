﻿using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
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

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUserNameAsync(username);
            if(user == null) return NotFound();

            _mapper.Map(memberUpdatesDto, user);

            if(await _userRepository.SaveAllAsync()) return NoContent();
            
            return BadRequest("Failed to update.");
        }

    }
}