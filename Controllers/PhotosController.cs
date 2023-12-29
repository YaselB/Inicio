using inicio.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Photo.Controller;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]

public class PhotosController : ControllerBase
{
    private readonly GaleriaContexto gallery;


    public PhotosController(GaleriaContexto galeria)
    {
        gallery = galeria;
    }
    [Consumes("multipart/form-data")]
    [HttpPost("{AlbumID}")]
    public async Task<ActionResult<Fotos>> PostFotosItem(int AlbumID, [FromForm] PhotosModel photosModel)
    {
        var find = await gallery.Album.FindAsync(AlbumID);
        if (find == null)
        {
            return NotFound("The album does not exist");
        }

        if (photosModel.Image == null || photosModel.Image.Length == 0)
        {
            return BadRequest("No image provided");
        }
        var filename = Guid.NewGuid().ToString() + ".jpg";
        var ruta = $"Files/{filename}";
        Fotos newFoto = new Fotos
        {
            AlbumID = AlbumID,
            album = find,
            Descripcion = photosModel.Descripcion,
            Fecha_Subida = DateTime.UtcNow,
            Titulo = photosModel.Titulo,
            Url_Imagen = ruta
        };
        await gallery.Fotos.AddAsync(newFoto);
        await gallery.SaveChangesAsync();
        using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await photosModel.Image.CopyToAsync(stream);
        }

        return Ok("Photo successfully registered");

    }
    [HttpGet("{PhotoID}")]
    public async Task<IActionResult> GetPhotoById(int PhotoID)
    {
        var find = await gallery.Fotos.FindAsync(PhotoID);
        if (find == null)
        {
            return NotFound("The Photo does not exist");
        }
        var ruta = find.Url_Imagen;
        try
        {
            var image = System.IO.File.ReadAllBytes(ruta);
            return File(image, "image/jpeg");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpGet("GetAll,{AlbumId}")]
    public async Task<IActionResult> GetAllPictures(int AlbumId)
    {
        var album = await gallery.Album.FindAsync(AlbumId);
        if (album == null)
        {
            return NotFound("The Album does not exist");
        }
        var urls = gallery.Fotos.Where(option => option.AlbumID == AlbumId).Select(option => new GetPhotos
        {
            ID = option.PhothosID,
            Url_Imagen = option.Url_Imagen
        }).ToList();
        if (urls == null)
        {
            return NotFound("The album has no photos");
        }
        return Ok(urls);
    }
    [HttpPatch("{PhotosID}")]
    public async Task<IActionResult> ChangePhoto(int PhotosID, string atribute, string newValue)
    {
        var find = await gallery.Fotos.FindAsync(PhotosID);
        if (find == null)
        {
            return NotFound("The Photo does not exist");
        }
        switch (atribute.ToLower())
        {
            case "descripcion":
                find.Descripcion = newValue;
                gallery.Entry(find).State = EntityState.Modified;
                await gallery.SaveChangesAsync();
                break;
            case "titulo":
                find.Titulo = newValue;
                gallery.Entry(find).State = EntityState.Modified;
                await gallery.SaveChangesAsync();
                break;
        }
        return Ok("Album successfully modified");
    }
    [HttpDelete("{PhotosID}")]
    public async Task<IActionResult> DeletePictures(int PhotosID)
    {
        var find = await gallery.Fotos.FindAsync(PhotosID);
        if (find == null)
        {
            return NotFound("The photo does not exist");
        }
        var ruta = find.Url_Imagen;
        if (System.IO.File.Exists(ruta))
        {
            System.IO.File.Delete(ruta);
        }
        gallery.Fotos.Remove(find);
        await gallery.SaveChangesAsync();
        return Ok("Photo succesfully deleted");
    }

}