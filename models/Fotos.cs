using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace inicio.models;

public class Fotos
{

   [Key]
   public int PhothosID { get; set; }
   public int AlbumID {get ;set ;}
   public required Album album{get ; set ;}

   public required string Titulo { get; set; }
   public required string Descripcion { get; set; }
   public required string Url_Imagen { get; set; }
   public required DateTime Fecha_Subida { get; set; }
   
   
}
