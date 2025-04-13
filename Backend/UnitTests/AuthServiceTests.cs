using Moq;
using Backend.Data;
using Backend.Repository.Interface;
using Backend.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
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
using Backend.Models.Tenancy;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

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
        private Mock<ITenancyResolver> _tenancyResolver;
        private Mock<IEmailService> _emailService;
        private Mock<ITemplateService> _templateService;
        private Mock<SignInManager<ApplicationUser>> _signInManager;
        private Mock<ISopService> _sopService;
        private ApplicationDbContext _dbContext;
        private AuthService _authService;
        private ModelStateDictionary _modelState;

        [SetUp]
        public void Setup()
        {
            // Setup HttpContextAccessor mock
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            // Setup TenancyResolver Mock
            _tenancyResolver = new Mock<ITenancyResolver>();

            // Setup in-memory database for ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options, _httpContextAccessorMock.Object, _tenancyResolver.Object);

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
            _emailService = new Mock<IEmailService>();
            _templateService = new Mock<ITemplateService>();
            _sopService = new Mock<ISopService>();

            var userManager = _userManagerMock.Object;
            var contextAccessor = _httpContextAccessorMock.Object;
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object;
            var options2 = new Mock<IOptions<IdentityOptions>>().Object;
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>().Object;
            var schemes = new Mock<IAuthenticationSchemeProvider>().Object;
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>().Object;

            _signInManager = new Mock<SignInManager<ApplicationUser>>(
                userManager,
                contextAccessor,
                claimsFactory,
                options2,
                logger,
                schemes,
                confirmation);

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
                appSettings,
                _tenancyResolver.Object,
                _emailService.Object,
                _templateService.Object,
                _signInManager.Object,
                _sopService.Object
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
            _dbContext.Database.EnsureDeleted();
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

            var applicationUser = new ApplicationUser()
            {
                UserName = request.Email,
            };

            _dbContext.ApplicationUsers.Add(applicationUser);
            await _dbContext.SaveChangesAsync();

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

            var applicationUser = new ApplicationUser()
            {

            };
            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _authService.SignupOrganisation(request, _modelState));
            Assert.That(exception.Message, Is.EqualTo("Password does not meet minimum requirements"));

        }

        [Test]
        public async Task Login_WithValidCredentials_ShouldSucceed()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO()
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var applicationUser = new ApplicationUser()
            {
                Id = "userId",
                Email = loginRequest.Email,
                UserName = loginRequest.Email
            };

            _dbContext.ApplicationUsers.Add(applicationUser);
            await _dbContext.SaveChangesAsync();

            var roles = new List<string> { StaticDetails.Role_Admin };
            var expectedToken = "test-jwt-token";
            var expectedRefreshToken = "test-refresh-token";
            AuthenticationResult authResult = new AuthenticationResult()
            {
                RefreshToken = expectedRefreshToken,
                Token = expectedToken
            };

            _unitOfWorkMock.Setup(uow => uow.ApplicationUsers.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(applicationUser);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(applicationUser, loginRequest.Password))
                .ReturnsAsync(true);

            _userManagerMock.Setup(um => um.GetRolesAsync(applicationUser))
                .ReturnsAsync(roles);

            _jwtServiceMock.Setup(jwt => jwt.GenerateAuthToken(applicationUser, roles))
                .Returns(Task.FromResult(authResult));

            _signInManager.Setup(x => x.PasswordSignInAsync(applicationUser, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Success));

            // Act
            var result = await _authService.Login(loginRequest, _modelState);

            // Assert
            Assert.That(result.IsSuccess, Is.True);

        }

        [Test]
        public async Task Login_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO()
            {
                Email = "invalid@example.com",
                Password = "Password.01!"
            };

            _unitOfWorkMock.Setup(uow => uow.ApplicationUsers.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _authService.Login(loginRequest, _modelState));
            Assert.That(exception.Message, Is.EqualTo("Email or password is incorrect"));
        }

        [Test]
        public async Task Login_WithInvalidPassword_ShouldThrowException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO()
            {
                Email = "test@example.com",
                Password = "InvalidPassword123!"
            };

            var applicationUser = new ApplicationUser()
            {
                Id = "userId",
                Email = loginRequest.Email,
                UserName = loginRequest.Email
            };

            _unitOfWorkMock.Setup(uow => uow.ApplicationUsers.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(applicationUser);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(applicationUser, loginRequest.Password))
                .ReturnsAsync(false);

            // Act & Assert

            var exception = Assert.ThrowsAsync<Exception>(async () => await _authService.Login(loginRequest, _modelState));
            Assert.That(exception.Message, Is.EqualTo("Email or password is incorrect"));

        }


    }
}