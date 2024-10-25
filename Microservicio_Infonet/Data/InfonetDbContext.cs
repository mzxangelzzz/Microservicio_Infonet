using Microservicio_Infonet.Models;
using Microsoft.EntityFrameworkCore;

public class InfonetDbContext : DbContext
{
    public InfonetDbContext(DbContextOptions<InfonetDbContext> options) : base(options)
    {
    }

    public DbSet<ClienteInfonet> clientes { get; set; }
}
