using Chronos.Data.Repositories.Resources;
using Chronos.Domain.Constraints;
using Chronos.Engine.Constraints.Evaluation.Validators;
using Chronos.Tests.Engine.TestFixtures;

namespace Chronos.Tests.Engine.Validators;

[TestFixture]
[Category("Unit")]
public class ActivityTypeCompatibilityValidatorTests
{
    private ActivityTypeCompatibilityValidator _validator = null!;
    private IResourceTypeRepository _resourceTypeRepository = null!;
    private ILogger<ActivityTypeCompatibilityValidator> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _resourceTypeRepository = Substitute.For<IResourceTypeRepository>();
        _logger = Substitute.For<ILogger<ActivityTypeCompatibilityValidator>>();
        _validator = new ActivityTypeCompatibilityValidator(_resourceTypeRepository, _logger);
    }

    [Test]
    public void ConstraintKey_ShouldBeCompatibleResourceTypes()
    {
        // Assert
        _validator.ConstraintKey.Should().Be("compatible_resource_types");
    }

    [Test]
    public async Task ValidateAsync_WhenResourceTypeIsCompatible_ShouldReturnNull()
    {
        // Arrange
        var resourceTypeId = Guid.NewGuid();
        var resourceType = TestDataBuilder.CreateResourceType(
            id: resourceTypeId,
            type: "Lecture Hall"
        );

        _resourceTypeRepository.GetByIdAsync(resourceTypeId).Returns(resourceType);

        var activity = TestDataBuilder.CreateActivity(activityType: "Lecture");
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(resourceTypeId: resourceTypeId);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "compatible_resource_types",
            value: "Lecture Hall,Seminar Room,Laboratory"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenResourceTypeIsNotCompatible_ShouldReturnViolation()
    {
        // Arrange
        var resourceTypeId = Guid.NewGuid();
        var resourceType = TestDataBuilder.CreateResourceType(
            id: resourceTypeId,
            type: "Computer Lab"
        );

        _resourceTypeRepository.GetByIdAsync(resourceTypeId).Returns(resourceType);

        var activity = TestDataBuilder.CreateActivity(activityType: "Lecture");
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(resourceTypeId: resourceTypeId);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "compatible_resource_types",
            value: "Lecture Hall,Seminar Room"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("Computer Lab");
        result.Message.Should().Contain("not compatible");
    }

    [Test]
    public async Task ValidateAsync_CaseInsensitive_ShouldMatch()
    {
        // Arrange
        var resourceTypeId = Guid.NewGuid();
        var resourceType = TestDataBuilder.CreateResourceType(
            id: resourceTypeId,
            type: "lecture hall"
        );

        _resourceTypeRepository.GetByIdAsync(resourceTypeId).Returns(resourceType);

        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(resourceTypeId: resourceTypeId);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "compatible_resource_types",
            value: "LECTURE HALL,SEMINAR ROOM"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenResourceTypeNotFound_ShouldReturnViolation()
    {
        // Arrange
        var resourceTypeId = Guid.NewGuid();

        _resourceTypeRepository
            .GetByIdAsync(resourceTypeId)
            .Returns((Domain.Resources.ResourceType?)null);

        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(resourceTypeId: resourceTypeId);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "compatible_resource_types",
            value: "Lecture Hall"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("not found");
    }

    [Test]
    public async Task ValidateAsync_EmptyValue_ShouldReturnViolation()
    {
        // Arrange
        var resourceTypeId = Guid.NewGuid();
        var resourceType = TestDataBuilder.CreateResourceType(
            id: resourceTypeId,
            type: "Lecture Hall"
        );

        _resourceTypeRepository.GetByIdAsync(resourceTypeId).Returns(resourceType);

        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(resourceTypeId: resourceTypeId);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "compatible_resource_types",
            value: ""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("No compatible resource types specified");
    }

    [Test]
    public async Task ValidateAsync_WithWhitespace_ShouldHandleCorrectly()
    {
        // Arrange
        var resourceTypeId = Guid.NewGuid();
        var resourceType = TestDataBuilder.CreateResourceType(
            id: resourceTypeId,
            type: "Seminar Room"
        );

        _resourceTypeRepository.GetByIdAsync(resourceTypeId).Returns(resourceType);

        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(resourceTypeId: resourceTypeId);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "compatible_resource_types",
            value: " Lecture Hall , Seminar Room , Laboratory "
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }
}
