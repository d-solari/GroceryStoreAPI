using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using GroceryStoreAPI.Dtos;
using GroceryStoreAPI.Models;
using GroceryStoreAPI.Data;
using AutoMapper;
using System.Linq;

namespace GroceryStoreAPI.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IGroceryStoreAPIRepo _repository;
        private readonly IMapper _mapper;

        public CustomersController(IGroceryStoreAPIRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReadDto>>> GetAllCustomers()
        {
            var customerItems = await _repository.GetAllCustomers();

            return Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(customerItems));
        }

        [HttpGet("{id}", Name = "GetCustomerById")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerById(int id)
        {
            var customer = await _repository.GetCustomerById(id);
            
            if(customer == null) return NotFound();

            return Ok(_mapper.Map<CustomerReadDto>(customer));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerReadDto>> CreateCustomer(CustomerCreateDto customerCreateDto)
        {
            var customerModel = _mapper.Map<Customer>(customerCreateDto);
            
            await _repository.CreateCustomer(customerModel);
            
            var customerReadDto = _mapper.Map<CustomerReadDto>(customerModel);

            return CreatedAtRoute(nameof(GetCustomerById), new { customerReadDto.Id }, customerReadDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, CustomerUpdateDto customerUpdateDto)
        {
            var customerModelFromRepo = await _repository.GetCustomerById(id);
            
            if (customerModelFromRepo == null) return NotFound();

            _mapper.Map(customerUpdateDto, customerModelFromRepo);

            await _repository.UpdateCustomer(customerModelFromRepo);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialCustomerUpdate(int id, JsonPatchDocument<CustomerUpdateDto> patchDocument)
        {
            var customerModelFromRepo = await _repository.GetCustomerById(id);

            if (customerModelFromRepo == null) return NotFound();

            var customerToPatch = _mapper.Map<CustomerUpdateDto>(customerModelFromRepo);
            
            patchDocument.ApplyTo(customerToPatch, ModelState);

            if (!TryValidateModel(customerToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(customerToPatch, customerModelFromRepo);

            await _repository.UpdateCustomer(customerModelFromRepo);

            return NoContent();
        }
    }
}
