using Chronos.Domain.Constraints;
using Chronos.Engine.Constraints.Evaluation.Validators;
using Chronos.Tests.Engine.TestFixtures;

namespace Chronos.Tests.Engine.Validators;

[TestFixture]
[Category("Unit")]
public class TimeRangeValidatorTests
{
    private TimeRangeValidator _validator = null!;
    private ILogger<TimeRangeValidator> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<TimeRangeValidator>>();
        _validator = new TimeRangeValidator(_logger);
    }

    [Test]
    public void ConstraintKey_ShouldBeTimeRange()
    {
        // Assert
        _validator.ConstraintKey.Should().Be("time_range");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotWithinRange_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            fromTime: new TimeSpan(9, 0, 0),
            toTime: new TimeSpan(10, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "time_range",
            value: "{\"start\": \"08:00\", \"end\": \"17:00\"}"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotStartsBeforeRange_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            fromTime: new TimeSpan(7, 0, 0),
            toTime: new TimeSpan(9, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "time_range",
            value: "{\"start\": \"08:00\", \"end\": \"17:00\"}"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("outside allowed range");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotEndsAfterRange_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            fromTime: new TimeSpan(16, 0, 0),
            toTime: new TimeSpan(18, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "time_range",
            value: "{\"start\": \"08:00\", \"end\": \"17:00\"}"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }

    [Test]
    public async Task ValidateAsync_WhenSlotExactlyAtBoundaries_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            fromTime: new TimeSpan(8, 0, 0),
            toTime: new TimeSpan(17, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "time_range",
            value: "{\"start\": \"08:00\", \"end\": \"17:00\"}"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_InvalidJson_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "time_range",
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

    [Test]
    public async Task ValidateAsync_InvalidTimeFormat_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "time_range",
            value: "{\"start\": \"25:00\", \"end\": \"17:00\"}"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("Invalid time format");
    }

    [Test]
    public async Task ValidateAsync_MissingStartTime_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "time_range",
            value: "{\"end\": \"17:00\"}"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }
}
