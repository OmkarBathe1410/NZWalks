using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        // Action Method: Get All Regions
        // GET: https://localhost:<port>/api/Regions
        [HttpGet]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll()
        {
            // Get data from database => Domain Models:
            var regionsDomain = await regionRepository.GetAllAsync();

            // Return DTOs
            return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
        }

        // Action Method: Get Region by Id
        // GET: https://localhost:<port>/api/Regions/id
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Get data from database => Domain Models:
            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            // Return DTOs
            return Ok(mapper.Map<RegionDto>(regionDomain));
        }

        // Action Method: Create a new Region
        // POST: https://localhost:<port>/api/Regions
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto regionRequestDto)
        {
            // Convert DTO into Domain Model:
            var regionDomainModel = mapper.Map<Region>(regionRequestDto);

            // Use Domain Model to create Region in DB & save changes::
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            // Map Domain Model back to DTO:
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            // Return Ok
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        // Action Method: Update existing Region
        // PUT: https://localhost:<port>/api/Regions/id
        [HttpPut("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Convert DTO into Domain Model:
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            // Update the region & save changes::
            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Return DTO:
            return Ok(mapper.Map<RegionDto>(regionDomainModel));

        }

        // Action Method: Delete existing Region
        // DELETE: https://localhost:<port>/api/Regions/id
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Delete the region & save changes:
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Return DTO:
            return Ok();
        }
    }
}
