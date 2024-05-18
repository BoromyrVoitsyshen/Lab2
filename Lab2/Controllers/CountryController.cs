using AutoMapper;
using Lab2.Dto;
using Lab2.Interfaces;
using Lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryInterface _countryInterface;
        private readonly IOwnerInterface _ownerInterface;
        private readonly IMapper _mapper;

        public CountryController(ICountryInterface countryInterface, IMapper mapper, IOwnerInterface ownerInterface)
        {
            _countryInterface = countryInterface;
            _mapper = mapper;
            _ownerInterface = ownerInterface;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]

        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryInterface.GetCountries());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryInterface.CountryExists(countryId))
                return NotFound();

            var country = _mapper.Map<CountryDto>(_countryInterface.GetCountry(countryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("/owners/{countryId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnersFromACountry(int countryId)
        {
            var owners = _mapper.Map<List<OwnerDto>>(_countryInterface.GetOwnersFromACountry(countryId));

            if (!ModelState.IsValid) 
                return BadRequest();

            return Ok(owners);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            if (countryCreate == null)
                return BadRequest(ModelState);

            var country = _countryInterface.GetCountries()
                .Where(c => c.Name == countryCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();
            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryCreate);

            if (!_countryInterface.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updatedCountry)
        {
            if (updatedCountry == null)
                return BadRequest(ModelState);

            if (countryId != updatedCountry.Id)
                return BadRequest(ModelState);

            if (!_countryInterface.CountryExists(countryId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(updatedCountry);

            if (!_countryInterface.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryInterface.CountryExists(countryId))
                return NotFound();

            var countryToDelete = _countryInterface.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownersOfACountry = _countryInterface.GetOwnersFromACountry(countryId).ToList();

            if (!_ownerInterface.DeleteOwners(ownersOfACountry))
                ModelState.AddModelError("", "Something went wrong deleting owners from a country");

            if (!_countryInterface.DeleteCountry(countryToDelete))
                ModelState.AddModelError("", "Something went wrong deleting country");

            return Ok("Successfully deleted");
        }
    }
}
