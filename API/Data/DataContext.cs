﻿using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes{get;set;} 
    public DbSet<Message> Messages{get;set;}

    protected override void OnModelCreating(ModelBuilder builder){

        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
        .HasKey(k => new {k.SourceUserID,k.TargetUserID});

        builder.Entity<UserLike>()
        .HasOne(s=>s.SourceUser)
        .WithMany(l => l.LikedUsers)
        .HasForeignKey(s=>s.SourceUserID)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
        .HasKey(k => new {k.SourceUserID,k.TargetUserID});

        builder.Entity<UserLike>()
        .HasOne(s=>s.TargetUser)
        .WithMany(l => l.LikedByUser)
        .HasForeignKey(s=>s.TargetUserID)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
        .HasOne(s=>s.Recipient)
        .WithMany(m => m.MessageReceived)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
        .HasOne(s=>s.Sender)
        .WithMany(m => m.MessageSent)
        .OnDelete(DeleteBehavior.Restrict);
        
    }
}
