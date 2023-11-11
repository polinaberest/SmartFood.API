using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartFood.API.Controllers;
using SmartFood.Domain;
using SmartFood.Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Results;
using SmartFood.Domain.Models;

namespace SmartFood.API.Tests.Controllers
{
    [TestClass]
    public class ODataControllerBaseTests
    {
        private ApplicationDbContext dbContext;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            dbContext = new ApplicationDbContext(options);
        }

        [TestMethod]
        public void Get_ShouldReturnOkResultWithEntities()
        {
            // Arrange
            var controller = new ODataControllerBase<Dish>(dbContext);
            var dish = new Dish { Id = Guid.NewGuid(), Name = "Test Dish" };
            dbContext.Add(dish);
            dbContext.SaveChanges();

            // Act
            var result = controller.Get();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = (OkObjectResult)result;
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<Dish>));

            var entities = (IEnumerable<Dish>)okResult.Value;
            Assert.AreEqual(1, entities.Count());
        }

        [TestMethod]
        public async Task GetByKey_ShouldReturnOkResultWithEntity()
        {
            // Arrange
            var controller = new ODataControllerBase<Dish>(dbContext);
            var dish = new Dish { Id = Guid.NewGuid(), Name = "Test Dish" };
            dbContext.Add(dish);
            dbContext.SaveChanges();

            // Act
            var result = await controller.Get(dish.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = (OkObjectResult)result;
            Assert.IsInstanceOfType(okResult.Value, typeof(Dish));

            var entity = (Dish)okResult.Value;
            Assert.AreEqual(dish.Id, entity.Id);
        }

        [TestMethod]
        public async Task Post_ShouldReturnCreatedResultWithEntity()
        {
            // Arrange
            var controller = new ODataControllerBase<Dish>(dbContext);
            var dish = new Dish { Name = "New Dish" };

            // Act
            var result = await controller.Post(dish);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedODataResult<Dish>));

            var createdResult = (CreatedODataResult<Dish>)result;
            Assert.IsInstanceOfType(createdResult.Value, typeof(Dish));

            var createdEntity = (Dish)createdResult.Value;
            Assert.AreEqual(dish.Name, createdEntity.Name);

            // Verify that the entity is actually added to the database
            var dbEntity = await dbContext.Set<Dish>().FirstOrDefaultAsync(e => e.Id == createdEntity.Id);
            Assert.IsNotNull(dbEntity);
        }

        [TestMethod]
        public async Task Put_ShouldReturnUpdatedResultWithEntity()
        {
            // Arrange
            var controller = new ODataControllerBase<Dish>(dbContext);
            var existingDish = new Dish { Id = Guid.NewGuid(), Name = "Existing Dish" };
            dbContext.Add(existingDish);
            dbContext.SaveChanges();

            var updatedDish = new Dish { Id = existingDish.Id, Name = "Updated Dish" };

            // Act
            var result = await controller.Put(existingDish.Id, updatedDish);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UpdatedODataResult<Dish>));

            var updatedResult = (UpdatedODataResult<Dish>)result;
            Assert.IsInstanceOfType(updatedResult.Value, typeof(Dish));

            var updatedEntity = (Dish)updatedResult.Value;
            Assert.AreEqual(updatedDish.Name, updatedEntity.Name);

            // Verify that the entity is actually updated in the database
            var dbEntity = await dbContext.Set<Dish>().FindAsync(existingDish.Id);
            Assert.IsNotNull(dbEntity);
            Assert.AreEqual(updatedDish.Name, dbEntity.Name);
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNoContentResult()
        {
            // Arrange
            var controller = new ODataControllerBase<Dish>(dbContext);
            var dish = new Dish { Id = Guid.NewGuid(), Name = "Dish to Delete" };
            dbContext.Add(dish);
            dbContext.SaveChanges();

            // Act
            var result = await controller.Delete(dish.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            // Verify that the entity is actually deleted from the database
            var dbEntity = await dbContext.Set<Dish>().FindAsync(dish.Id);
            Assert.IsNull(dbEntity);
        }
    }
    
}
