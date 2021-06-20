using AutoMapper;
using GroceryStoreAPI.Controllers;
using GroceryStoreAPI.Data;
using GroceryStoreAPI.Dtos;
using GroceryStoreAPI.Models;
using GroceryStoreAPI.Profiles;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreAPITests
{
    public class GroceryStoreAPIControllerTest
    {
        private readonly CustomersController _controller;
        private readonly Mock<IGroceryStoreAPIRepo> _mockRepo;
        private readonly IMapper _mapper;

        public GroceryStoreAPIControllerTest()
        {
            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new CustomersProfile()));
            _mapper = mockMapper.CreateMapper();

            _mockRepo = new Mock<IGroceryStoreAPIRepo>();

            _controller = new CustomersController(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetAllCustomers_WhenCalled_ReturnsOkResult()
        {
            _mockRepo.Setup(repo => repo.GetAllCustomers().Result).Returns(new List<Customer>() { new Customer() { }, new Customer() { } });

            var okResult = await _controller.GetAllCustomers();

            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public async Task GetAllCustomers_WhenCalled_ReturnsAllItems()
        {
            _mockRepo.Setup(repo => repo.GetAllCustomers().Result).Returns(new List<Customer>() { new Customer() { }, new Customer() { } });

            var okResult = await _controller.GetAllCustomers();

            var customers = Assert.IsType<ActionResult<IEnumerable<CustomerReadDto>>>(okResult);
        }

        [Fact]
        public async Task GetCustomerById_ExistingIdPassed_ReturnsOkResult()
        {
            _mockRepo.Setup(repo => repo.GetCustomerById(It.IsAny<int>()).Result).Returns(new Customer() { });
            var result = await _controller.GetCustomerById(0);

            var okResult = result.Result as OkObjectResult;

            Assert.IsType<OkObjectResult>(okResult);
        }

        [Fact]
        public async Task GetCustomerById_NonExistingIdPassed_ReturnsNotFoundResult()
        {
            var result = await _controller.GetCustomerById(It.IsAny<int>());

            var notFoundResult = result.Result as NotFoundResult;

            Assert.IsType<NotFoundResult>(notFoundResult);
        }

        [Fact]
        public async Task GetCustomerById_ExistingIdPassed_ReturnsRightItem()
        {
            _mockRepo.Setup(repo => repo.GetCustomerById(It.IsAny<int>()).Result).Returns(new Customer() { Id = 1, Name = "Bob" });
            var result = await _controller.GetCustomerById(1);

            var okResult = result.Result as OkObjectResult;

            var value = okResult.Value as CustomerReadDto;

            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(1, value.Id);
        }

        [Fact]
        public async Task CreateCustomer_ValidObjectPassed_ReturnsCreatedResponse()
        {
            _mockRepo.Setup(repo => repo.CreateCustomer(new Customer() { }));

            var createdResponse = await _controller.CreateCustomer(new CustomerCreateDto() { });

            var result = createdResponse.Result as CreatedAtRouteResult;

            Assert.IsType<CreatedAtRouteResult>(result);
        }

        [Fact]
        public async Task CreateCustomer_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            _mockRepo.Setup(repo => repo.CreateCustomer(new Customer() { }));

            var createdResponse = await _controller.CreateCustomer(new CustomerCreateDto() { Id = 1, Name = "Bob" });
            var result = createdResponse.Result as CreatedAtRouteResult;
            var value = result.Value as CustomerReadDto;

            Assert.IsType<CustomerReadDto>(value);
            Assert.Equal("Bob", value.Name);
        }

        [Fact]
        public async Task UpdateCustomer_NonExistingIdPassed_ReturnedNotFoundResult()
        {
            _mockRepo.Setup(repo => repo.UpdateCustomer(new Customer() { }));

            var updatedResponse = await _controller.UpdateCustomer(It.IsAny<int>(), new CustomerUpdateDto() { Name = "Bob" });

            Assert.IsType<NotFoundResult>(updatedResponse);
        }

        [Fact]
        public async Task UpdateCustomer_ExistingIdPassed_ReturnedNoContentResult()
        {
            var customer = new Customer() { Id = 1, Name = "Bob" };
            _mockRepo.Setup(repo => repo.GetCustomerById(It.IsAny<int>()).Result).Returns(customer);
            _mockRepo.Setup(repo => repo.UpdateCustomer(customer));

            var updatedResponse = await _controller.UpdateCustomer(It.IsAny<int>(), new CustomerUpdateDto() { Name = "Bob" });

            Assert.IsType<NoContentResult>(updatedResponse);
        }

        [Fact]
        public async Task PartialCustomerUpdate_NonExistingIdPassed_ReturnedNotFoundResult()
        {
            var jsonObject = new JsonPatchDocument<CustomerUpdateDto>();   
            jsonObject.Replace(c => c.Name, "");

            var notFoundResult = await _controller.PartialCustomerUpdate(It.IsAny<int>(), jsonObject);

            Assert.IsType<NotFoundResult>(notFoundResult);
        }

        [Fact]
        public async Task PartialCustomerUpdate_ExistingIdPassed_WhenModelStateIsInvalid()
        {
            var jsonObject = new JsonPatchDocument<CustomerUpdateDto>();
            jsonObject.Replace(c => c.Name, "David");

            var customer = new Customer() { Id = 1, Name = "Bob" };
            _mockRepo.Setup(repo => repo.GetCustomerById(It.IsAny<int>()).Result).Returns(customer);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            _controller.ObjectValidator = objectValidator.Object;
            

            var updatedResponse = await _controller.PartialCustomerUpdate(1, jsonObject);

            Assert.IsType<NoContentResult>(updatedResponse);
        }
    }
}
