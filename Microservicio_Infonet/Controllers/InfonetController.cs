using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microservicio_Infonet.Models;  // Importar el namespace correcto

[Route("api/[controller]")]
[ApiController]
public class InfonetController : ControllerBase
{
    private readonly InfonetDbContext _context;
    private static readonly string[] Entidades = new[]
    {
        "Banco del Sol", "Finanzas XYZ", "Credito Fácil", "Préstamos Seguros", "Fondo Solidario"
    };

    public InfonetController(InfonetDbContext context)
    {
        _context = context;
    }

    // Método para registrar un cliente con datos enviados desde Gestion_Prestamos
    [HttpPost("registrar-cliente")]
    public async Task<IActionResult> RegistrarCliente([FromBody] ClienteGestionDto clienteDto)
    {
        // Validación básica de los datos recibidos
        if (clienteDto == null || string.IsNullOrEmpty(clienteDto.Dpi) || string.IsNullOrEmpty(clienteDto.Nombre))
        {
            return BadRequest(new { Message = "Datos del cliente incompletos" });
        }

        // Crear una nueva instancia de ClienteInfonet usando los datos recibidos de Gestion_Prestamos
        ClienteInfonet clienteInfonet = new ClienteInfonet
        {
            dpi = clienteDto.Dpi,
            nombre = clienteDto.Nombre,
            apellido = clienteDto.Apellido,
        };

        // Generar aleatoriamente si el cliente tiene deuda
        Random random = new Random();
        bool tieneDeuda = random.Next(0, 2) == 1; // 50% probabilidad de tener deuda

        if (tieneDeuda)
        {
            // Asignar entidad, monto de deuda y fecha de vencimiento aleatoriamente
            clienteInfonet.estado_credito = "Deuda";
            clienteInfonet.deuda = random.Next(1000, 50000); // Montos entre 1000 y 50000
            clienteInfonet.entidad = Entidades[random.Next(Entidades.Length)];
            clienteInfonet.fecha_vencimiento = DateTime.UtcNow.AddMonths(-random.Next(1, 24)); // Fecha entre 1 y 24 meses atrás
        }
        else
        {
            // Cliente está limpio
            clienteInfonet.estado_credito = "Limpio";
            clienteInfonet.deuda = 0;
            clienteInfonet.entidad = "N/A";
            clienteInfonet.fecha_vencimiento = null;
        }

        // Guardar en la base de datos de Infonet
        _context.clientes.Add(clienteInfonet);
        await _context.SaveChangesAsync();

        // Retornar el cliente registrado en Infonet
        return CreatedAtAction(nameof(GetClientePorDpi), new { dpi = clienteInfonet.dpi }, clienteInfonet);
    }

    // Método para obtener un cliente por DPI
    [HttpGet("buscar-cliente/{dpi}")]
    public async Task<ActionResult<ClienteInfonet>> GetClientePorDpi(string dpi)
    {
        var cliente = await _context.clientes.FirstOrDefaultAsync(c => c.dpi == dpi);

        if (cliente == null)
        {
            return NotFound(new { Message = "Cliente no encontrado en Infonet" });
        }

        return Ok(cliente);
    }

    // Método para obtener datos desde Gestion_Prestamos usando HttpClient (si fuera necesario usarlo directamente)
    private async Task<ClienteGestionDto> ObtenerDatosDesdeGestionPrestamos(string dpi)
    {
        using (HttpClient client = new HttpClient())
        {
            // Supongamos que el endpoint del backend de Gestion_Prestamos es el siguiente:
            string url = $"https://url-gestion-prestamos/api/clientes/{dpi}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ClienteGestionDto>(jsonResponse);
            }
            return null;
        }
    }
}

// DTO para recibir datos desde Gestion_Prestamos
public class ClienteGestionDto
{
    public string Dpi { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
}
