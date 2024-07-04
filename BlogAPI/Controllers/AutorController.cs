using AutoMapper;
using BlogAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Dto;
using SharedModels;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly IAutorRepository _autorRepository;
        private readonly ILogger<AutorController> _logger;
        private readonly IMapper _mapper;

        public AutorController(IAutorRepository studentRepo, 
            ILogger<AutorController> logger, IMapper mapper)
        {
            _autorRepository = studentRepo;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AutorDto>>> GetAutores()
        {
            try
            {
                _logger.LogInformation("Obteniendo los Autores");

                var autors = await _autorRepository.GetAllAsync();

                return Ok(_mapper.Map<IEnumerable<AutorDto>>(autors));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener los Autores: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener los Autores");
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AutorDto>> GetAutor(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"ID de Autor no válido: {id}");
                return BadRequest("ID de Autor no válido");
            }

            try
            {
                _logger.LogInformation($"Obteniendo Autor con ID: {id}");

                var autor = await _autorRepository.GetByIdAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning($"No se encontró ningún Autor con ID: {id}");
                    return NotFound("Autor no encontrado.");
                }

                return Ok(_mapper.Map<AutorDto>(autor));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener estudiante con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener el estudiante.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AutorDto>> PostAutor(AutorCreateDto createDto)
        {
            if (createDto == null)
            {
                _logger.LogError("Se recibió un Autor nulo en la solicitud.");
                return BadRequest("El Autor no puede ser nulo.");
            }

            try
            {
                _logger.LogInformation($"Creando un nuevo Autor con nombre: {createDto.Name}");

                // Verificar si el estudiante ya existe
                var existingAutor = await _autorRepository
                    .GetAsync(s => s.Name != null && s.Name.ToLower()
                    == createDto.Name.ToLower());

                if (existingAutor != null)
                {
                    _logger.LogWarning($"El Autor con nombre '{createDto.Name}' ya existe.");
                    ModelState.AddModelError("NombreExiste", "¡El Autor con ese nombre ya existe!");
                    return BadRequest(ModelState);
                }

                // Verificar la validez del modelo
                if (!ModelState.IsValid)
                {
                    _logger.LogError("El modelo de Autor no es válido.");
                    return BadRequest(ModelState);
                }

                // Crear el nuevo estudiante
                var newAutor = _mapper.Map<Autor>(createDto);

                await _autorRepository.CreateAsync(newAutor);

                _logger.LogInformation($"Nuevo Autor '{createDto.Name}' creado con ID: " +
                    $"{newAutor.AutorId}");
                return CreatedAtAction(nameof(GetAutor), new { id = newAutor.AutorId }, newAutor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear un nuevo estudiante: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al crear un nuevo estudiante.");
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAutor(int id, AutorUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.AutorId)
            {
                return BadRequest("Los datos de entrada no son válidos " +
                    "o el ID del Autor no coincide.");
            }

            try
            {
                _logger.LogInformation($"Actualizando Autor con ID: {id}");

                var existingAutor = await _autorRepository.GetByIdAsync(id);
                if (existingAutor == null)
                {
                    _logger.LogWarning($"No se encontró ningún Autor con ID: {id}");
                    return NotFound("El Autor no existe.");
                }

                // Actualizar solo las propiedades necesarias del Autor existente
                _mapper.Map(updateDto, existingAutor);

                await _autorRepository.SaveChangesAsync();

                _logger.LogInformation($"Autor con ID {id} actualizado correctamente.");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _autorRepository.ExistsAsync(s => s.AutorId == id))
                {
                    _logger.LogWarning($"No se encontró ningún Autor con ID: {id}");
                    return NotFound("El Autor no se encontró durante la actualización.");
                }
                else
                {
                    _logger.LogError($"Error de concurrencia al actualizar el Autor " +
                        $"con ID: {id}. Detalles: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error interno del servidor al actualizar el Autor.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Autor con ID {id}: " +
                    $"{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al actualizar el Autor.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            try
            {
                _logger.LogInformation($"Eliminando Autor con ID: {id}");

                var autor = await _autorRepository.GetByIdAsync(id);
                if (autor == null)
                {
                    _logger.LogWarning($"No se encontró ningún Autor con ID: {id}");
                    return NotFound("Autor no encontrado.");
                }

                await _autorRepository.DeleteAsync(autor);

                _logger.LogInformation($"Autor con ID {id} eliminado correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Autor con ID {id}: " +
                    $"{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Se produjo un error al eliminar el Autor.");
            }
        }
    }
}
