﻿using API.Entities;

namespace API;

public class AppUser
{
    public int Id {get; set;}
    public string UserName {get; set;}
    public byte[] PassWordHash { get; set; }
    public byte[] PassWordSalt { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string Gender { get; set; } 
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; } 
    public string City { get; set; }
    public string Country { get; set; }
    public List<Photo> Photos { get; set; } = new();
}
