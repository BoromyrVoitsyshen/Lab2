using Microsoft.AspNetCore.Mvc;
using Lab2.Interfaces;
using Lab2.Models;
using AutoMapper;
using Lab2.Dto;
using System.Collections.Generic;

namespace Lab2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareController : Controller
    {
        private readonly IWareInterface _wareInterface;
        private readonly IOwnerInterface _ownerInterface;
        private readonly ICategoryInterface _categoryInterface;
        private readonly IReviewInterface _reviewInterface;
        private readonly IMapper _mapper;

        public WareController(IOwnerInterface ownerInterface, ICategoryInterface categoryInterface, 
            IWareInterface wareInterface, IMapper mapper, IReviewInterface reviewInterface)
        {
            _wareInterface = wareInterface;
            _ownerInterface = ownerInterface;
            _categoryInterface = categoryInterface;
            _reviewInterface = reviewInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Ware>))]

        public IActionResult GetWares()
        {
            var wares = _mapper.Map<List<WareDto>>(_wareInterface.GetWares());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(wares);
        }

        [HttpGet("{wareId}")]
        [ProducesResponseType(200, Type = typeof(Ware))]
        [ProducesResponseType(400)]
        public IActionResult GetWare(int wareId)
        {
            if (!_wareInterface.WareExists(wareId))
                return NotFound();

            var ware = _mapper.Map<WareDto>(_wareInterface.GetWare(wareId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(ware);
        }

        [HttpGet("{wareId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetWareRating(int wareId)
        {
            if (!_wareInterface.WareExists(wareId))
                return NotFound();

            var rating = _wareInterface.GetWareRating(wareId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateWare([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] WareDto wareCreate)
        {
            if (wareCreate == null || !_ownerInterface.OwnerExists(ownerId) || !_categoryInterface.CategoryExists(categoryId))
                return BadRequest(ModelState);

            var ware = _wareInterface.GetWares()
                .Where(r => r.Name == wareCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();
            if (ware != null)
            {
                ModelState.AddModelError("", "Ware already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var wareMap = _mapper.Map<Ware>(wareCreate);

            if (!_wareInterface.CreateWare(ownerId, categoryId, wareMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{wareId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateWare(int wareId, [FromQuery] int ownerId,
            [FromQuery] int categoryId, [FromBody] WareDto updatedWare)
        {
            if (updatedWare == null
                || !ModelState.IsValid
                || wareId != updatedWare.Id)
                return BadRequest(ModelState);

            if (!_wareInterface.WareExists(wareId) 
                || !_ownerInterface.OwnerExists(ownerId) 
                || !_categoryInterface.CategoryExists(categoryId))
                return NotFound();

            var wareMap = _mapper.Map<Ware>(updatedWare);

            if (!_wareInterface.UpdateWare(ownerId, categoryId, wareMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{wareId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteWare(int wareId)
        {
            if (!_wareInterface.WareExists(wareId))
                return NotFound();

            var wareToDelete = _wareInterface.GetWare(wareId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsOfAWare = _reviewInterface.GetReviewsOfAWare(wareId).ToList();

            if(!_reviewInterface.DeleteReviews(reviewsOfAWare))
                ModelState.AddModelError("", "Something went wrong deleting reviews of a ware");

            if (!_wareInterface.DeleteWare(wareToDelete))
                    ModelState.AddModelError("", "Something went wrong deleting ware");

            return Ok("Successfully deleted");
        }
    }
}
