using AutoMapper;
using BlogAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels.Dto;
using SharedModels;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicacionController : ControllerBase
    {
        private readonly IPublicacionRepository _publicacionRepository;
        private readonly IAutorRepository _autorRepository;
        private readonly ILogger<PublicacionController> _logger;
        private readonly IMapper _mapper;

        public PublicacionController(IAutorRepository studentRepo, IPublicacionRepository publicacionRepo,
            ILogger<PublicacionController> logger, IMapper mapper)
        {
            _autorRepository = studentRepo;
            _publicacionRepository = publicacionRepo;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PublicacionDto>>> GetPublicaciones()
        {
            try
            {
                _logger.LogInformation("Obteniendo las Publicaciones");

                var publicaciones = await _publicacionRepository.GetAllAsync();

                return Ok(_mapper.Map<IEnumerable<PublicacionDto>>(publicaciones));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener las Publicaciones: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener las Publicaciones");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublicacionDto>> GetPublicacion(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"ID de Publicacion no válida: {id}");
                return BadRequest("ID de Publicacion no válida");
            }

            try
            {
                _logger.LogInformation($"Obteniendo Publicacion con ID: {id}");

                var publicacion = await _publicacionRepository.GetByIdAsync(id);

                if (publicacion == null)
                {
                    _logger.LogWarning($"No se encontró ninguna Publicacion con ID: {id}");
                    return NotFound("Publicacion no encontrada.");
                }

                return Ok(_mapper.Map<PublicacionDto>(publicacion));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener Publicacion con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener la Publicacion.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublicacionDto>> PostPublicacion(PublicacionCreateDto createDto)
        {
            if (createDto == null)
            {
                _logger.LogError("Se recibió una Publicacion nula en la solicitud.");
                return BadRequest("La Publicacion no puede ser nula.");
            }

            try
            {
                _logger.LogInformation($"Creando un nueva Publicacion para el Autor con " +
                    $"ID: {createDto.AutorId} en la fecha: {createDto.CreatedAt}");

                // Verificar si el Autor existe
                var autorExists = await _autorRepository
                    .ExistsAsync(s => s.AutorId == createDto.AutorId);

                if (!autorExists)
                {
                    _logger.LogWarning($"El Autor con ID '{createDto.AutorId}' no existe.");
                    ModelState.AddModelError("AutorNoExiste", "¡El Autor no existe!");
                    return BadRequest(ModelState);
                }

                // Verificar si la Publicacion ya existe para la fecha y el Autor
                var existingAttendance = await _publicacionRepository
                    .GetAsync(a => a.AutorId == createDto.AutorId
                    && a.CreatedAt == createDto.CreatedAt);

                if (existingAttendance != null)
                {
                    _logger.LogWarning($"La Publicacion para el Autor con ID " +
                        $"'{createDto.AutorId}' ya existe para la fecha '{createDto.CreatedAt}'");
                    ModelState.AddModelError("PublicacionExiste",
                        "¡La Publicacion para esa fecha ya existe!");
                    return BadRequest(ModelState);
                }

                // Verificar la validez del modelo
                if (!ModelState.IsValid)
                {
                    _logger.LogError("El modelo de Publicacion no es válido.");
                    return BadRequest(ModelState);
                }

                // Crear la nueva Publicacion
                var newPublicacion = _mapper.Map<Publicacion>(createDto);

                await _publicacionRepository.CreateAsync(newPublicacion);

                _logger.LogInformation($"Nueva Publicacion creada con ID: " +
                    $"{newPublicacion.PublicacionId}");
                return CreatedAtAction(nameof(GetPublicacion),
                    new { id = newPublicacion.PublicacionId }, newPublicacion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear una nueva Publicacion: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al crear una nueva Publicacion.");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPublicacion(int id,
            PublicacionUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.PublicacionId)
            {
                return BadRequest("Los datos de entrada no son válidos " +
                    "o el ID de la Publicacion no coincide.");
            }

            try
            {
                _logger.LogInformation($"Actualizando Publicacion con ID: {id}");

                var existingPublicacion = await _publicacionRepository.GetByIdAsync(id);
                if (existingPublicacion == null)
                {
                    _logger.LogWarning($"No se encontró ninguna Publicacion con ID: {id}");
                    return NotFound("La Publicacion no existe.");
                }

                // Verificar si el estudiante existe
                var autorExists = await _autorRepository
                    .ExistsAsync(s => s.AutorId == updateDto.AutorId);

                if (!autorExists)
                {
                    _logger.LogWarning($"El Autor con ID '{updateDto.AutorId}' no existe.");
                    ModelState.AddModelError("AutorNoExiste", "¡El Autor no existe!");
                    return BadRequest(ModelState);
                }

                // Actualizar solo las propiedades necesarias de la Publicacion existente
                _mapper.Map(updateDto, existingPublicacion);

                await _publicacionRepository.SaveChangesAsync();

                _logger.LogInformation($"Publicacion con ID {id} actualizada correctamente.");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _publicacionRepository.ExistsAsync(a => a.PublicacionId == id))
                {
                    _logger.LogWarning($"No se encontró ninguna Publicacion con ID: {id}");
                    return NotFound("La Publicacion no se encontró durante la actualización.");
                }
                else
                {
                    _logger.LogError($"Error de concurrencia al actualizar la Publicacion " +
                        $"con ID: {id}. Detalles: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error interno del servidor al actualizar la Publicacion.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la Publicacion con ID {id}: " +
                    $"{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al actualizar la Publicacion.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            try
            {
                _logger.LogInformation($"Eliminando Publicacion con ID: {id}");

                var attendance = await _publicacionRepository.GetByIdAsync(id);
                if (attendance == null)
                {
                    _logger.LogWarning($"No se encontró ninguna Publicacion con ID: {id}");
                    return NotFound("Publicacion no encontrada.");
                }

                await _publicacionRepository.DeleteAsync(attendance);

                _logger.LogInformation($"Publicacion con ID {id} eliminada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la Publicacion con ID {id}: " +
                    $"{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Se produjo un error al eliminar la Publicacion.");
            }
        }
    }
}
