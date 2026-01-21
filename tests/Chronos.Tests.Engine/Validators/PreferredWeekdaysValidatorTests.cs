using Chronos.Domain.Constraints;
using Chronos.Domain.Schedule;
using Chronos.Engine.Constraints.Evaluation.Validators;
using Chronos.Tests.Engine.TestFixtures;

namespace Chronos.Tests.Engine.Validators;

[TestFixture]
[Category("Unit")]
public class PreferredWeekdaysValidatorTests
{
    private PreferredWeekdaysValidator _validator = null!;
    private ILogger<PreferredWeekdaysValidator> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<PreferredWeekdaysValidator>>();
        _validator = new PreferredWeekdaysValidator(_logger);
    }

    [Test]
    public void ConstraintKey_ShouldBePreferredWeekdays()
    {
        // Assert
        _validator.ConstraintKey.Should().Be("preferred_weekdays");
    }

    [Test]
    public async Task ValidateAsync_WhenWeekdayMatches_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(weekday: "Monday");
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_weekdays",
            value: "Monday,Wednesday,Friday"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenWeekdayDoesNotMatch_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(weekday: "Tuesday");
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_weekdays",
            value: "Monday,Wednesday,Friday"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ConstraintKey.Should().Be("preferred_weekdays");
        result.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
        result.Message.Should().Contain("Tuesday");
        result.Message.Should().Contain("not in preferred weekdays");
    }

    [Test]
    public async Task ValidateAsync_CaseInsensitive_ShouldMatch()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(weekday: "monday");
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_weekdays",
            value: "MONDAY,WEDNESDAY"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WithWhitespace_ShouldHandleCorrectly()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(weekday: "Wednesday");
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_weekdays",
            value: " Monday , Wednesday , Friday "
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_EmptyValue_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(weekday: "Monday");
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(key: "preferred_weekdays", value: "");

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_SingleWeekday_ShouldWork()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(weekday: "Friday");
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_weekdays",
            value: "Friday"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenExceptionOccurs_ShouldReturnHardViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(weekday: "Monday");
        var resource = TestDataBuilder.CreateResource();
        // Create a constraint with null value to trigger exception
        var constraint = new ActivityConstraint
        {
            Id = Guid.NewGuid(),
            ActivityId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Key = "preferred_weekdays",
            Value = null! // This will cause NullReferenceException when calling .Split()
        };

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ConstraintKey.Should().Be("preferred_weekdays");
        result.ConstraintValue.Should().BeNull();
        result.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Be("Invalid constraint format");
        result.Details.Should().NotBeNullOrEmpty(); // Should contain exception message
    }
}
