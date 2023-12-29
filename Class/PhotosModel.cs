namespace inicio.models;
public class PhotosModel{
    public required string Titulo { get; set; }
   public required string Descripcion { get; set; }
   public required IFormFile Image {get; set ;}
}