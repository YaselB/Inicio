
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using inicio.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace inicio.Controller;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{

    private readonly GaleriaContexto gallery;
    private readonly IEmailService emailService;
    private readonly UserService userService;
    private readonly IGeneratedJwt generatedJwt;
    public readonly IConfiguration configuration;



    public UserController(GaleriaContexto contexto, IEmailService emailService2, UserService userService2, IGeneratedJwt generatedJwt2, IConfiguration configuration2)
    {
        gallery = contexto;
        emailService = emailService2;
        userService = userService2;
        generatedJwt = generatedJwt2;
        configuration = configuration2;
    }

    [HttpPost]
    public async Task<ActionResult<Usuarios>> PostUsuariosItem([FromBody] UserModel userModel)
    {
        var find = await gallery.Usuarios.FirstOrDefaultAsync(User => User.Email == userModel.Email);
        if (find != null)
        {
            return NotFound("Email not available");
        }
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-]+$";
        Match match = Regex.Match(userModel.Email, pattern);
        if (!match.Success)
        {
            return NotFound("Please enter a valid email");
        }
        string pattern2 = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$";
        Match match1 = Regex.Match(userModel.Password, pattern2);
        if (!match1.Success)
        {
            if (!Regex.IsMatch(userModel.Password, "(?=.*[A-Z])"))
            {
                return BadRequest("The password must have at least one uppercase letter");
            }
            else if (!Regex.IsMatch(userModel.Password, "(?=.*[a-z])"))
            {
                return BadRequest("The password must have at least one lowercase letter");
            }
            else if (!Regex.IsMatch(userModel.Password, "(?=.*\\d)"))
            {
                return BadRequest("The password must have numbers");
            }
            else if (!Regex.IsMatch(userModel.Password, "(?=.*[^\\da-zA-Z])"))
            {
                return BadRequest("The password must contain special characters");
            }
            else
            {
                return BadRequest("The password must have 6 or more characters");
            }
        }

        Usuarios usuarios = new Usuarios
        {
            Email = userModel.Email,
            Password = userService.GeneratePassword(userModel.Password),
            confirm = false,
            token = emailService.SendEmailAsync(userModel.Email, "http://localhost:5214/api/User/confirm?token="),
            albums = null,
            rols = null

        };

        await gallery.Usuarios.AddAsync(usuarios);
        await gallery.SaveChangesAsync();

        return Ok("Please confirm your email");
    }
    [HttpPost("PostRole,{id}")]
    public async Task<IActionResult> PostUserRol(int id, string? clave)
    {
        var find = await gallery.Usuarios.FindAsync(id);
        if (find == null)
        {
            return NotFound("User not registered");
        }
        var userrole = await gallery.rols.FirstOrDefaultAsync(option => option.UserID == id);
        if (userrole != null)
        {
            return BadRequest("The user have a rol : " + userrole.rol);
        }
        if (clave != null)
        {
            if (clave.Equals("MiBro*1528#"))
            {
                var rolAdmin = new Rol
                {
                    usuarios = find,
                    UserID = id,
                    rol = "Admin"
                };
                await gallery.rols.AddAsync(rolAdmin);
                await gallery.SaveChangesAsync();
                return Ok("You are a : " + rolAdmin.rol);
            }
            var rolUser = new Rol
            {
                usuarios = find,
                UserID = id,
                rol = "User"
            };
            await gallery.rols.AddAsync(rolUser);
            await gallery.SaveChangesAsync();
            return Ok("You are a : " + rolUser.rol);
        }
        var rolUser2 = new Rol
        {
            usuarios = find,
            UserID = id,
            rol = "User"

        };
        await gallery.rols.AddAsync(rolUser2);
        await gallery.SaveChangesAsync();
        return Ok("You are a : " + rolUser2.rol);

    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Usuarios>> GetUsuariosItem(int id)
    {
        Usuarios? findUser = await gallery.Usuarios.FindAsync(id);
            if (findUser == null)
            {
                return NotFound("User not exist");
            }
        var role = gallery.rols.Where(option => option.UserID == findUser.DNI).Select(option => option.rol).FirstOrDefault();
        if (role == null)
        {
            return NotFound("the user does not have a role");
        }
        if (!role.Equals("Admin"))
        {
            return StatusCode(403, "The user does not have permission for this action");
        }
            
            return findUser;
        
        

    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> PutUsuariosItem(int id, string password)
    {

        Usuarios? find = await gallery.Usuarios.FindAsync(id);
        if (find == null)
        {
            return NotFound("User not found");
        }


        string pattern2 = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$";
        Match match1 = Regex.Match(password, pattern2);
        if (!match1.Success)
        {
            if (!Regex.IsMatch(password, "(?=.*[A-Z])"))
            {
                return BadRequest("The password must have at least one uppercase letter");
            }
            else if (!Regex.IsMatch(password, "(?=.*[a-z])"))
            {
                return BadRequest("The password must have at least one lowercase letter");
            }
            else if (!Regex.IsMatch(password, "(?=.*\\d)"))
            {
                return BadRequest("The password must have numbers");
            }
            else if (!Regex.IsMatch(password, "(?=.*[^\\da-zA-Z])"))
            {
                return BadRequest("The password must contain special characters");
            }
            else
            {
                return BadRequest("The password must have 6 or more characters");
            }
        }
        find.Password = userService.GeneratePassword(password);
        find.confirm = false;
        find.token = emailService.SendEmailAsync(find.Email, "http://localhost:5214/api/User/confirm?token=");
        gallery.Entry(find).State = EntityState.Modified;
        await gallery.SaveChangesAsync();
        return Ok("Password succesfully changed , now Please confirm your Email");
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
        var user = await gallery.Usuarios.FirstOrDefaultAsync(n => n.token == token);
        if (user == null)
        {
            return NotFound("Invalid confirmation token");
        }
        user.confirm = true;
        user.token = null;
        gallery.Entry(user).State = EntityState.Modified;
        await gallery.SaveChangesAsync();
        return Ok("Email is confirmed");
    }
    [Authorize]
    [HttpGet("GetAll,{email}")]
    public IActionResult GetUsers(string email)
    {
        var find = gallery.Usuarios.FirstOrDefault(option => option.Email == email);
        if (find == null)
        {
            return NotFound("User not registered");
        }
        var role = gallery.rols.Where(option => option.UserID == find.DNI).Select(option => option.rol).FirstOrDefault();
        if (role == null)
        {
            return NotFound("the user does not have a role");
        }
        if (!role.Equals("Admin"))
        {
            return StatusCode(403, "The user does not have permission for this action");
        }
        List<Usuarios> nuevo = gallery.Usuarios.ToList();
        if (nuevo == null)
        {
            return NotFound("No registered users");
        }
        return Ok(nuevo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuariosItem(int id)
    {
        var find = await gallery.Usuarios.FindAsync(id);
        if (find == null)
        {
            return NotFound("User not found");
        }
        var findAlbum = gallery.Album.Where(option => option.UserId == find.DNI).ToList();
        if (findAlbum != null)
        {
            foreach (Album i in findAlbum)
            {
                var findphoto = gallery.Fotos.Where(option => option.AlbumID == i.ID).ToList();
                if (findphoto != null)
                {
                    foreach(var j in findphoto){
                        if(System.IO.File.Exists(j.Url_Imagen)){
                          System.IO.File.Delete(j.Url_Imagen);  
                        }
                    }
                    gallery.Fotos.RemoveRange(findphoto);
                    await gallery.SaveChangesAsync();
                }
                gallery.Album.Remove(i);
                await gallery.SaveChangesAsync();
            }
        }
        gallery.Usuarios.Remove(find);
        await gallery.SaveChangesAsync();
        return Ok("User successfully deleted");
    }
    [HttpPost("login")]
    public async Task<IActionResult> UserLogin([FromForm] UserModel userModel)
    {
        var confirm =await gallery.Usuarios.FirstOrDefaultAsync(option =>option.Email == userModel.Email);
        if(confirm== null){
        return NotFound("User not registered");
        } 
        if(!confirm.confirm){
            return BadRequest("Please confirm your email");
        }
        var storedHashedPassword = gallery.Usuarios.FirstOrDefault(options => options.Email == userModel.Email);
        if (storedHashedPassword != null)
        {
            bool isPasswordValid = userService.verifyPassword(userModel.Password, storedHashedPassword.Password);

            if (isPasswordValid)
            {
                storedHashedPassword.token = generatedJwt.GeneratedToken(storedHashedPassword.Email, storedHashedPassword.Password);
                gallery.Entry(storedHashedPassword).State = EntityState.Modified;
                await gallery.SaveChangesAsync();
                return Ok(new { storedHashedPassword.token });
            }
        }
        return BadRequest("User not authorize");
    }
    [HttpPost("Autenticate")]
    public async Task<IActionResult> UserAutenticate([FromBody] UserAutenticate userAutenticate)
    {
        Usuarios? find = gallery.Usuarios.FirstOrDefault(options => options.Email == userAutenticate.Email);
        if (find == null)
        {
            return NotFound("User not registered");
        }

        if (generatedJwt.VerifyToken(userAutenticate.Token))
        {
            return Unauthorized("Please login again");
        }
        find.token = generatedJwt.GeneratedToken(find.Email, find.Password);
        gallery.Entry(find).State = EntityState.Modified;
        await gallery.SaveChangesAsync();
        return Ok("User is Autenticate");
    }


}

