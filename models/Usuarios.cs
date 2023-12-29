using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace inicio.models;

public class Usuarios
{
   [Key]
   public int DNI { get; set; }
   public required string Email { get; set; }
   public required string Password { get; set; }

   public required bool confirm {get ; set ;}

   public required string? token {get  ; set ;} 
   public required ICollection<Album>? albums  {get ; set ;}
   public required Rol? rols {get; set ;}
   
}