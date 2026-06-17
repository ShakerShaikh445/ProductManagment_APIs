using Azure.Core;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagment_APIs.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PagedRequest request)
        {
            return Ok(await _repository.GetAllAsync(request));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var result = await _repository.CreateAsync(dto);

            if (!result)
                return BadRequest("Username already exists.");

            return Ok("User Created Successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            var result = await _repository.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return Ok("User Updated Successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("User Deleted Successfully");
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _repository.ChangeStatusAsync(id);

            if (!result)
                return NotFound();

            return Ok("Status Updated Successfully");
        }
    }
}