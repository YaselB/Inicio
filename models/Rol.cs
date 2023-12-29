namespace inicio.models;
public class Rol{
    public int ID {get ; set ;}
    public required Usuarios usuarios {get ; set ;}
    public required int UserID {get ; set ;}
    public required string rol {get ; set ;}
}