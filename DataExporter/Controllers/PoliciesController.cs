using DataExporter.Dtos;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DataExporter.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly PolicyService _policyService;
        private readonly ILogger<PoliciesController> _logger;

        public PoliciesController(PolicyService policyService, ILogger<PoliciesController> logger)
        {
            _policyService = policyService;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ReadPolicyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostPolicies([FromBody] CreatePolicyDto createPolicyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var readPolicyDto = await _policyService.CreatePolicyAsync(createPolicyDto);

                if (readPolicyDto == null)
                {
                    _logger.LogError("Policy creation failed. CreatePolicyAsync returned null.");
                    return BadRequest("Policy could not be created.");
                }

                return CreatedAtAction(nameof(GetPolicy), new { id = readPolicyDto.Id }, readPolicyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred while creating the record");
            }
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IList<ReadPolicyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPolicies()
        {
            try
            {
                var policies = await _policyService.ReadPoliciesAsync();
                return Ok(policies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred while retrieving policies");
            }
        }
        
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReadPolicyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReadPolicyDto>> GetPolicy(int id)
        {
            try
            {
                var policyDto = await _policyService.ReadPolicyAsync(id);

                if (policyDto == null)
                {
                    _logger.LogInformation($"Policy with ID '{id}' not found.");
                    return NotFound();
                }

                return Ok(policyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred while retrieving the record");
            }
        }

        /// <summary>
        /// Exports policies within a given date range along with their notes.
        /// </summary>
        /// <param name="startDate">The start date for filtering policies.</param>
        /// <param name="endDate">The end date for filtering policies.</param>
        /// <returns>A list of policies and their notes matching the criteria.</returns>
        [HttpGet("export")]
        [ProducesResponseType(typeof(IList<ExportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportPolicies([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (!DateTime.TryParseExact(startDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate) ||
                    !DateTime.TryParseExact(endDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate))
                {
                    _logger.LogInformation($"Policy dates were supplied in this format - startDate:{startDate} - endDate:{endDate} - but must be in yyyy-MM-dd format");
                    return BadRequest("Dates must be in yyyy-MM-dd format.");
                }

                if (fromDate > toDate)
                {
                    _logger.LogInformation($"Start date cannot be after end date - startDate:{startDate} - endDate:{endDate}");
                    return BadRequest("Start date cannot be after end date.");
                }

                var result = await _policyService.ExportPoliciesAsync(fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An error occurred while processing the Export request");
            }
        }
    }
}
