﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
    {
        var query = _context.Messages.OrderByDescending(x=>x.MessageSent).AsQueryable();

        query = messageParams.Container switch{
            "Inbox" => query.Where(u=> u.RecipientUsername == messageParams.Username && !u.RecipientDeleted),
            "Outbox" => query.Where(u=>u.SenderUsername == messageParams.Username && !u.SenderDeleted),
            _=> query.Where(u=>u.RecipientUsername == messageParams.Username && !u.RecipientDeleted && u.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        var messages = await _context.Messages
            .Include(u=>u.Sender).ThenInclude(p=>p.Photos)
            .Include(r=>r.Recipient).ThenInclude(p=>p.Photos)
            .Where(
                m=> m.RecipientUsername == currentUserName && m.SenderUsername == recipientUserName && !m.RecipientDeleted
                || m.RecipientUsername == recipientUserName && m.SenderUsername == currentUserName && !m.SenderDeleted
            ).OrderBy(m=>m.MessageSent).ToListAsync();

            var unreadMessages = messages.Where(m=>m.DateRead == null && m.RecipientUsername == currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
