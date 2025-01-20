using NUnit.Framework;
using Moq;
using Backend.Data;
using Backend.Repository.Interface;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Backend.Service.Interface;
using Backend.Service.Implementation;
using Backend.Models.Settings;
using Microsoft.Extensions.Configuration;
using Backend.Models.Dto;
using Backend.Utility;
using System.Net;

namespace Backend.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<IJwtService> _jwtServiceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private ApplicationDbContext _dbContext;
        private AuthService _authService;
        private ModelStateDictionary _modelState;

        [SetUp]
        public void Setup()
        {
            // Setup HttpContextAccessor mock
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            // Setup in-memory database for ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options, _httpContextAccessorMock.Object);

            // Setup UserManager mock
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Setup RoleManager mock
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null, null, null, null);

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _jwtServiceMock = new Mock<IJwtService>();
            _modelState = new ModelStateDictionary();

            // Setup identity options
            var identityOptions = Options.Create(new IdentityOptions());
            var appSettings = Options.Create(new ApplicationSettings());

            _authService = new AuthService(
                _dbContext,
                Mock.Of<IConfiguration>(),
                _roleManagerMock.Object,
                _userManagerMock.Object,
                _jwtServiceMock.Object,
                identityOptions,
                _unitOfWorkMock.Object,
                appSettings
            );

            // Setup transaction mock
            _unitOfWorkMock
                .Setup(uow => uow.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns((Func<Task> action) => action());

            // Setup SaveAsync mock
            _unitOfWorkMock
                .Setup(uow => uow.SaveAsync())
                .Returns(Task.CompletedTask);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task SignupOrganisation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var request = new OrganisationSignupRequest
            {
                OrganisationName = "Test Org",
                Email = "test@example.com",
                Password = "Test123!",
                Forename = "John",
                Surname = "Doe"
            };

            // Setup mocks for successful path
            _unitOfWorkMock
                .Setup(uow => uow.ApplicationUsers.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((ApplicationUser)null);

            _unitOfWorkMock
                .Setup(uow => uow.Organisations.GetAsync(It.IsAny<Expression<Func<Organisation, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((Organisation)null);

            _roleManagerMock
                .Setup(rm => rm.RoleExistsAsync(StaticDetails.Role_Admin))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), StaticDetails.Role_Admin))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.SignupOrganisation(request, _modelState);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.SuccessMessage, Is.EqualTo("Organisation and user created successfully"));

            // Verify interactions
            _unitOfWorkMock.Verify(
                uow => uow.Organisations.AddAsync(It.Is<Organisation>(o =>
                    o.Name == request.OrganisationName)),
                Times.Once);

            _userManagerMock.Verify(
                um => um.CreateAsync(It.Is<ApplicationUser>(u =>
                    u.Email == request.Email.ToLower() &&
                    u.Forename == request.Forename &&
                    u.Surname == request.Surname),
                    request.Password),
                Times.Once);

            _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.AtLeastOnce);
        }

        [Test]
        public async Task SignupOrganisation_WithExistingUser_ShouldThrowException()
        {
            // Arrange
            var request = new OrganisationSignupRequest
            {
                OrganisationName = "Test Org",
                Email = "existing@example.com",
                Password = "Test123!",
                Forename = "John",
                Surname = "Doe"
            };

            _unitOfWorkMock
                .Setup(uow => uow.ApplicationUsers.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new ApplicationUser { Email = request.Email });

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _authService.SignupOrganisation(request, _modelState));

            Assert.That(exception.Message, Is.EqualTo("User already exists"));
        }

        [Test]
        public async Task SignupOrganisation_WithExistingOrganisation_ShouldThrowException()
        {
            // Arrange
            var request = new OrganisationSignupRequest
            {
                OrganisationName = "Existing Org",
                Email = "test@example.com",
                Password = "Test123!",
                Forename = "John",
                Surname = "Doe"
            };

            _unitOfWorkMock
                .Setup(uow => uow.ApplicationUsers.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((ApplicationUser)null);

            _unitOfWorkMock
                .Setup(uow => uow.Organisations.GetAsync(It.IsAny<Expression<Func<Organisation, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Organisation { Name = request.OrganisationName });

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _authService.SignupOrganisation(request, _modelState));

            Assert.That(exception.Message, Is.EqualTo("Organisation already exists"));
        }

        [Test]
        public async Task SignupOrganisation_WithInvalidModelState_ShouldThrowException()
        {
            // Arrange
            var request = new OrganisationSignupRequest();
            _modelState.AddModelError("Email", "Email is required");

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _authService.SignupOrganisation(request, _modelState));

            Assert.That(exception.Message, Is.EqualTo("Email is required"));
        }

        [Test]
        [TestCase("tooSh0rt")]
        [TestCase("noDigitsButLong")]
        [TestCase("NOLOWERCASE1")]
        [TestCase("noupper1case")]
        [TestCase("nnnnnnnnnn")]
        public async Task SignupOrganisation_WithInvalidPassword_ShouldThrowException(string password)
        {
            // Arrange
            var request = new OrganisationSignupRequest()
            {
                OrganisationName = "Test Org",
                Email = "test@example.com",
                Password = password,
                Forename = "John",
                Surname = "Doe"
            };


            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _authService.SignupOrganisation(request, _modelState));
            Assert.That(exception.Message, Is.EqualTo("Password does not meet minimum requirements"));

        }
    }
}