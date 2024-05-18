using AutoMapper;
using Lab2.Dto;
using Lab2.Interfaces;
using Lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerInterface _ownerInterface;
        private readonly ICountryInterface _countryInterface;
        private readonly IWareInterface _wareInterface;
        private readonly IMapper _mapper;
        public OwnerController(IOwnerInterface ownerInterface, ICountryInterface countryInterface, IMapper mapper, IWareInterface wareInterface)
        {
            _ownerInterface = ownerInterface;
            _countryInterface = countryInterface;
            _mapper = mapper;
            _wareInterface = wareInterface;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerInterface.GetOwners());

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]

        public IActionResult GetOwner(int ownerId)
        {
            if(!_ownerInterface.OwnerExists(ownerId))
                return NotFound();

            var owners = _mapper.Map<OwnerDto>(_ownerInterface.GetOwner(ownerId));

            if(!ModelState.IsValid) 
                return BadRequest();

            return Ok(owners);
        }

        [HttpGet("{ownerId}/ware")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetWareByOwner(int ownerId)
        {
            if(!_ownerInterface.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<List<WareDto>>(_ownerInterface.GetWareByOwner(ownerId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            if (ownerCreate == null || !_countryInterface.CountryExists(countryId))
                return BadRequest(ModelState);

            var owner = _ownerInterface.GetOwners()
                .Where(o => o.Name == ownerCreate.Name.TrimEnd().ToUpper()
                && o.Country == _countryInterface.GetCountry(countryId)).FirstOrDefault();

            if (owner != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(ownerCreate);

            ownerMap.Country = _countryInterface.GetCountry(countryId);

            if (!_ownerInterface.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwner)
        {
            if (updatedOwner == null)
                return BadRequest(ModelState);

            if (ownerId != updatedOwner.Id)
                return BadRequest(ModelState);

            if (!_ownerInterface.OwnerExists(ownerId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(updatedOwner);

            if (!_ownerInterface.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerInterface.OwnerExists(ownerId))
                return NotFound();

            var ownerToDelete = _ownerInterface.GetOwner(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var waresOfAOwner = _ownerInterface.GetWareByOwner(ownerId).ToList();

            if (!_wareInterface.DeleteWares(waresOfAOwner))
                ModelState.AddModelError("", "Something went wrong deleting wares of a owner");

            if (!_ownerInterface.DeleteOwner(ownerToDelete))
                ModelState.AddModelError("", "Something went wrong deleting owner");

            return Ok("Successfully deleted");
        }
    }
}
