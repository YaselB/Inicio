using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace inicio.models;

public class GaleriaContexto : DbContext
{
    public GaleriaContexto(DbContextOptions<GaleriaContexto> options) : base(options)
    {
    }
    public DbSet<Usuarios>Usuarios{get ; set ;}
    public DbSet<Fotos> Fotos { get; set; }
    public DbSet<Album> Album { get; set; }
    public DbSet<Rol>rols {get ; set ;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Album>().HasOne(f => f.usuarios).WithMany(a=> a.albums).HasForeignKey(F => F.UserId);
        modelBuilder.Entity<Fotos>().HasOne(f => f.album).WithMany(a => a.fotos).HasForeignKey(f => f.AlbumID);
        modelBuilder.Entity<Rol>().HasOne(f => f.usuarios).WithOne(a => a.rols).HasForeignKey<Rol>(f => f.UserID);
    }
}
