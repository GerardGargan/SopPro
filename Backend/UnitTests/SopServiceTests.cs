using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using Azure.Storage.Blobs;
using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Settings;
using Backend.Models.Tenancy;
using Backend.Repository.Interface;
using Backend.Service.Implementation;
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
        private SopService sopService;

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

            sopService = new SopService(_unitOfWorkMock.Object, _tenancyResolver.Object, _dbContext, _blobService.Object, appSettings, _emailService.Object, _templateService.Object, _userManagerMock.Object, _chatService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task CreateSop_WithValidData_ShouldSucceed()
        {
            // Arrange
            var organisationId = 1;
            var userId = Guid.NewGuid().ToString();

            var model = new SopDto
            {
                Title = "Test SOP",
                Description = "This is a test description",
                Reference = "Ref123",
                isAiGenerated = false,
                DepartmentId = 1,
                SopHazards = new List<SopHazardDto>
                {
                    new SopHazardDto { Name = "Hazard 1", ControlMeasure = "Measure 1", RiskLevel = RiskLevel.Medium }
                },
                SopSteps = new List<SopStepDto>
                {
                    new SopStepDto { Title = "Step 1", Text = "Do something", Position = 1 }
                }
            };

            // Mock GetAsync to return null (no existing SOP)
            _unitOfWorkMock.Setup(uow => uow.Sops.GetAsync(It.IsAny<Expression<Func<Sop, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((Sop)null);

            // Mock tenancy
            _tenancyResolver.Setup(t => t.GetOrganisationid()).Returns(organisationId);
            _tenancyResolver.Setup(t => t.GetUserId()).Returns(userId);

            // Setup transaction method to just run the action
            _unitOfWorkMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(func => func());

            // Setup mocks for AddAsync and SaveAsync
            _unitOfWorkMock.Setup(u => u.Sops.AddAsync(It.IsAny<Sop>()))
                .Callback<Sop>(s => s.Id = 1) // Simulate DB setting the Id
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SopVersions.AddAsync(It.IsAny<SopVersion>()))
                .Callback<SopVersion>(v => v.Id = 1)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SopHazards.AddRangeAsync(It.IsAny<IEnumerable<SopHazard>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SopSteps.AddRangeAsync(It.IsAny<IEnumerable<SopStep>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var response = await sopService.CreateSop(model);

            // Assert
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That("Sop created successfully", Is.EqualTo(response.SuccessMessage));
        }

        [Test]
        public async Task CreateSop_WithInvalidTitle_ShouldThrowException()
        {
            // Arrange
            var model = new SopDto
            {
                Title = " ",
                Description = "This is a test description",
                Reference = "Ref123",
                isAiGenerated = false,
                DepartmentId = 1,
            };

            _unitOfWorkMock.Setup(uow => uow.Sops.GetAsync(It.IsAny<Expression<Func<Sop, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((Sop)null);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await sopService.CreateSop(model));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Title cant be empty"));

        }

        [Test]
        public async Task CreateSop_WithInvalidDescription_ShouldThrowException()
        {
            //Arrange
            var model = new SopDto
            {
                Title = "Test title",
                Description = " ",
                Reference = "Ref123",
                isAiGenerated = false,
                DepartmentId = 1,
            };

            _unitOfWorkMock.Setup(uow => uow.Sops.GetAsync(It.IsAny<Expression<Func<Sop, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((Sop)null);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await sopService.CreateSop(model));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Description cant be empty"));

        }

        [Test]
        public async Task GetSops_ShouldBeFiltered_ByOrganisationId()
        {
            // Arrange
            int orgId = 1;
            int nonMatchingOrgId = 2;

            // Create HttpContext with authentication and claims
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim("organisationId", orgId.ToString()),
                new Claim("id", "test-user-id")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            httpContext.User = principal;

            // Setup HttpContextAccessor to return our context with claims
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Now the tenancy resolver should be able to get the organisation ID from claims
            // No need to mock the GetOrganisationid method as it will use the real implementation

            // Add test data to the in memory database
            _dbContext.Sops.Add(new Sop { Id = 1, OrganisationId = orgId, Reference = "sop-a" });
            _dbContext.Sops.Add(new Sop { Id = 2, OrganisationId = nonMatchingOrgId, Reference = "sop-b" });
            await _dbContext.SaveChangesAsync();

            // Clear the change tracker to ensure fresh queries
            _dbContext.ChangeTracker.Clear();

            // Act - query should be filtered by the orgId from tenancy resolver
            var result = await _dbContext.Sops.ToListAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Reference, Is.EqualTo("sop-a"));
        }

        [Test]
        public async Task ApproveSop_WithDraftStatus_ShouldSucceed()
        {
            // Arrange
            var sopId = 1;
            var sop = new Sop
            {
                Id = sopId,
                Reference = "SOP-1234",
                SopVersions = new List<SopVersion>
                {
                    new SopVersion
                    {
                        Version = 1,
                        Status = SopStatus.Draft,
                        Title = "SOP Title",
                        AuthorId = "author-id",
                        ApprovedById = "approver-id",
                        ApprovalDate = DateTime.UtcNow
                    }
                }
            };

            var author = new ApplicationUser
            {
                Id = "author-id",
                Forename = "Author",
                Surname = "Test",
                Email = "author@example.com"
            };

            var approver = new ApplicationUser
            {
                Id = "approver-id",
                Forename = "Approver",
                Surname = "Test"
            };

            // Mocking unit of work methods
            _unitOfWorkMock.Setup(uow => uow.Sops.GetAsync(It.IsAny<Expression<Func<Sop, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(sop);
            _unitOfWorkMock.Setup(uow => uow.SopVersions.GetAsync(It.IsAny<Expression<Func<SopVersion, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(sop.SopVersions.First());
            _unitOfWorkMock.Setup(uow => uow.ApplicationUsers.GetAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>())
            )
            .ReturnsAsync((Expression<Func<ApplicationUser, bool>> predicate, string includeProperties, bool asNoTracking) =>
            {
                var author = new ApplicationUser
                {
                    Id = "author-id",
                    Forename = "Author",
                    Surname = "Test",
                    Email = "author@example.com"
                };

                var approver = new ApplicationUser
                {
                    Id = "approver-id",
                    Forename = "Approver",
                    Surname = "Test"
                };

                var userId = predicate.Compile().ToString();

                if (userId == author.Id)
                {
                    return author;
                }

                return approver;
            });


            // Mocking template rendering
            _templateService.Setup(ts => ts.RenderTemplateAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync("Email body");

            // Mocking email sending
            _emailService.Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            // Act
            var result = await sopService.ApproveSop(sopId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.SuccessMessage, Is.EqualTo("Sop approved"));

            // Verify that the status of the sop is updated to Approved
            Assert.That(sop.SopVersions.First().Status, Is.EqualTo(SopStatus.Approved));

        }


    }
}