

using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;

namespace inicio.models;

public class UserModel
{

    public required string Email { get; set; }

    public required string Password { get; set; }
   
}