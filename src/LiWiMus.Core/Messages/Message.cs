﻿using LiWiMus.Core.Chats;

namespace LiWiMus.Core.Messages;

public class Message : BaseEntity
{
    public Chat Chat { get; set; } = null!;
    public User Sender { get; set; } = null!;
    public string Text { get; set; } = null!;
}