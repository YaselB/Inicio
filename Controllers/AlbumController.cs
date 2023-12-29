using inicio.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace inicio.Controller;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AlbumController : ControllerBase
{
    public readonly GaleriaContexto gallery;

    public AlbumController(GaleriaContexto contexto)
    {
        gallery = contexto;
    }
    [HttpPost("{UserID}")]
    public async Task<IActionResult> PostAlbum(int UserID,AlbumModel albumModel){
        var user = await gallery.Usuarios.FindAsync(UserID);
        if(user == null){
            return NotFound("Please enter a valid user");
        }
        if(!user.confirm){
            return BadRequest("Please confirm your gmail");
        }
        var album = new Album{
            Descripcion = albumModel.Descripcion,
            Name = albumModel.Name,
            fotos = null,
            UserId = UserID,
            usuarios = user
        };
        await gallery.Album.AddAsync(album);
        await gallery.SaveChangesAsync();
        return Ok("Album succesfully create");
    }
    [HttpGet("{userid}")]
    public async Task<IActionResult> GetAlBum(int userid){
        var user = await gallery.Usuarios.FindAsync(userid);
        if(user == null){
            return NotFound("Please enter a valid user");
        }
        if(!user.confirm){
            return BadRequest("Please confirm your gmail");
        }
        var nuevo = gallery.Album.Where(option =>option.UserId == userid)
                               .Select(album => new AlbumModel
                               {
                                  Descripcion = album.Descripcion,
                                  Name = album.Name
                                  
                               })
                               .ToList();
        if(nuevo == null){
            return NotFound("User haven't Albums");
        }
        
        return Ok(nuevo);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlbum(int id){
        var find = await gallery.Album.FindAsync(id);
        if(find == null){
        return NotFound("The album does not exist");
        }
        var find2 = gallery.Fotos.Where(option =>option.AlbumID == find.ID).ToList();
        if(find2 != null){
            foreach (var i in find2){
                if(System.IO.File.Exists(i.Url_Imagen)){
                 System.IO.File.Delete(i.Url_Imagen);   
                }
            }
            gallery.Fotos.RemoveRange(find2);
            await gallery.SaveChangesAsync();
        }
     gallery.Album.Remove(find);
     await gallery.SaveChangesAsync();
     return Ok("Album Succesfully Deleted");
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> ChangeAlbum(int id , string nameAtribute ,string newvalue){
        var find = await gallery.Album.FindAsync(id);
        if(find == null){
            return NotFound("The album does not exist");
        }
        switch(nameAtribute.ToLower()){
        case "descripcion":
        find.Descripcion = newvalue;
        gallery.Entry(find).State = EntityState.Modified;
        await gallery.SaveChangesAsync();
        break;
        case "name":
        find.Name = newvalue;
        gallery.Entry(find).State = EntityState.Modified;
        await gallery.SaveChangesAsync();
        break;
        }
        return Ok("Album successfully modified");
    }
}