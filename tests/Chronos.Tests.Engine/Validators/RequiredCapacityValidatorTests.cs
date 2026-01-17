using Chronos.Domain.Constraints;
using Chronos.Engine.Constraints.Evaluation.Validators;
using Chronos.Tests.Engine.TestFixtures;

namespace Chronos.Tests.Engine.Validators;

[TestFixture]
[Category("Unit")]
public class RequiredCapacityValidatorTests
{
    private RequiredCapacityValidator _validator = null!;
    private ILogger<RequiredCapacityValidator> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<RequiredCapacityValidator>>();
        _validator = new RequiredCapacityValidator(_logger);
    }

    [Test]
    public void ConstraintKey_ShouldBeRequiredCapacity()
    {
        // Assert
        _validator.ConstraintKey.Should().Be("required_capacity");
    }

    [Test]
    public async Task ValidateAsync_WhenCapacityMeetsMinimum_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(capacity: 50);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "required_capacity",
            value: """{"min": 30}"""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenCapacityBelowMinimum_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(capacity: 20);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "required_capacity",
            value: """{"min": 30}"""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("below minimum required");
    }

    [Test]
    public async Task ValidateAsync_WhenCapacityExceedsMaximum_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(capacity: 60);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "required_capacity",
            value: """{"min": 20, "max": 50}"""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("exceeds maximum allowed");
    }

    [Test]
    public async Task ValidateAsync_WhenCapacityWithinRange_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(capacity: 35);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "required_capacity",
            value: """{"min": 20, "max": 50}"""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenResourceHasNoCapacity_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(capacity: null);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "required_capacity",
            value: """{"min": 30}"""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("does not have capacity information");
    }

    [Test]
    public async Task ValidateAsync_WhenExpectedStudentsExceedsCapacity_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity(expectedStudents: 60);
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(capacity: 50);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "required_capacity",
            value: """{"min": 30}"""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("insufficient for expected students");
    }

    [Test]
    public async Task ValidateAsync_InvalidJson_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(capacity: 50);
        var constraint = TestDataBuilder.CreateConstraint(
            key: "required_capacity",
            value: "not valid json"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("Invalid JSON format");
    }
}
