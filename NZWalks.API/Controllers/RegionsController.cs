using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Action Method: Get All Regions
        // GET: https://localhost:<port>/api/Regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get data from database => Domain Models:
            var regionsDomain = await dbContext.Regions.ToListAsync();

            // Map Domain Models to DTOs:
            var regionsDto = new List<RegionDto>();

            foreach (var region in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl,
                });
            }

            // Return DTOs
            return Ok(regionsDto);
        }

        // Action Method: Get Region by Id
        // GET: https://localhost:<port>/api/Regions/id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Get data from database => Domain Models:
            var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            // Map Domain Models to DTOs:
            var regionDto = new RegionDto()
            {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };

            // Return DTOs
            return Ok(regionDto);
        }

        // Action Method: Create a new Region
        // POST: https://localhost:<port>/api/Regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto regionRequestDto)
        {
            // Convert DTO into Domain Model:
            var regionDomainModel = new Region()
            {
                Code = regionRequestDto.Code,
                Name = regionRequestDto.Name,
                RegionImageUrl = regionRequestDto.RegionImageUrl,
            };

            // Use Domain Model to create Region in DB:
            await dbContext.Regions.AddAsync(regionDomainModel);
            await dbContext.SaveChangesAsync();

            // Map Domain Model back to DTO:
            var regionDto = new RegionDto()
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl,
            };

            // Return Ok
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        // Action Method: Update existing Region
        // PUT: https://localhost:<port>/api/Regions/id
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Check if region exists:
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Map DTO to DomainModel:
            regionDomainModel.Code = updateRegionRequestDto.Code;
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            // Convert Domain Model to DTO:
            var regionDto = new RegionDto()
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl,
            };

            // Return DTO:
            return Ok(regionDto);

        }

        // Action Method: Delete existing Region
        // DELETE: https://localhost:<port>/api/Regions/id
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Check if region exists:
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Delete the region & save changes:
            dbContext.Regions.Remove(regionDomainModel);
            await dbContext.SaveChangesAsync();

            // Return DTO:
            return Ok();
        }
    }
}
