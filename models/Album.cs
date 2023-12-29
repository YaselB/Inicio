using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace inicio.models;

public class Album
{
    [Key]
    public int ID { get; set; }
    
    public required Usuarios usuarios {get ; set ;}
    public int UserId {get ; set ;}
    public required string Descripcion { get; set; }
    public required string Name { get; set; }
    public required ICollection<Fotos>? fotos {get ; set ;}   
}