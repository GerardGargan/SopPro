using Azure.Storage.Blobs;
using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Models.Settings;
using Backend.Models.Tenancy;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;

namespace Backend.Tests
{
    [TestFixture]
    public class SopServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ITenancyResolver> _tenancyResolver;
        private ApplicationDbContext _dbContext;
        private Mock<IBlobService> _blobService;
        private Mock<IEmailService> _emailService;
        private Mock<ITemplateService> _templateService;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IChatCompletionService> _chatService;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;

        [SetUp]
        public void Setup()
        {
            // Setup HttpContextAccessor mock
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

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

            // Unit of work mock
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Blob service mock
            _blobService = new Mock<IBlobService>();

            // App settings
            var appSettings = Options.Create(new ApplicationSettings());

            // Email service mock
            _emailService = new Mock<IEmailService>();

            // Template service mock
            _templateService = new Mock<ITemplateService>();

            // Chat service mock
            _chatService = new Mock<IChatCompletionService>();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

    }
}