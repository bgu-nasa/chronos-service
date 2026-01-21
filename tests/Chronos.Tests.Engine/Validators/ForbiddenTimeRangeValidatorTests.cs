using Chronos.Domain.Constraints;
using Chronos.Engine.Constraints.Evaluation.Validators;
using Chronos.Tests.Engine.TestFixtures;

namespace Chronos.Tests.Engine.Validators;

[TestFixture]
[Category("Unit")]
public class ForbiddenTimeRangeValidatorTests
{
    private ForbiddenTimeRangeValidator _validator = null!;
    private ILogger<ForbiddenTimeRangeValidator> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<ForbiddenTimeRangeValidator>>();
        _validator = new ForbiddenTimeRangeValidator(_logger);
    }

    [Test]
    public void ConstraintKey_ShouldBeForbiddenTimeRange()
    {
        // Assert
        _validator.ConstraintKey.Should().Be("forbidden_timerange");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotDoesNotOverlap_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(8, 0, 0),
            toTime: new TimeSpan(9, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotOverlaps_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(9, 0, 0),
            toTime: new TimeSpan(10, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
        result.Message.Should().Contain("overlaps with forbidden time range");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotCompletelyWithinForbiddenRange_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(10, 0, 0),
            toTime: new TimeSpan(10, 30, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }

    [Test]
    public async Task ValidateAsync_WhenSlotStartsBeforeAndEndsInForbiddenRange_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(9, 0, 0),
            toTime: new TimeSpan(10, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }

    [Test]
    public async Task ValidateAsync_WhenSlotStartsInAndEndsAfterForbiddenRange_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(10, 0, 0),
            toTime: new TimeSpan(12, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }

    [Test]
    public async Task ValidateAsync_WhenDifferentWeekday_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Tuesday",
            fromTime: new TimeSpan(9, 30, 0),
            toTime: new TimeSpan(11, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenMultipleForbiddenRanges_ShouldCheckAll()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Wednesday",
            fromTime: new TimeSpan(13, 0, 0),
            toTime: new TimeSpan(14, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00, Wednesday 13:00 - 15:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }

    [Test]
    public async Task ValidateAsync_WhenMultipleForbiddenRangesButNoOverlap_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Wednesday",
            fromTime: new TimeSpan(10, 0, 0),
            toTime: new TimeSpan(12, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30 - 11:00, Wednesday 13:00 - 15:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenEmptyValue_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: ""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenInvalidFormat_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "invalid format"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Invalid format entries are ignored, so if no valid ranges are found, returns null
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenInvalidTimeFormat_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 25:00 - 26:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Invalid time format entries are ignored, so if no valid ranges are found, returns null
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenStartTimeAfterEndTime_ShouldIgnoreInvalidEntry()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(9, 30, 0),
            toTime: new TimeSpan(11, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 11:00 - 09:00" // Invalid: start after end
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Should return null because invalid entry is ignored
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenCaseInsensitiveWeekday_ShouldMatch()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(9, 30, 0),
            toTime: new TimeSpan(11, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "MONDAY 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }

    [Test]
    public async Task ValidateAsync_WhenNoSpaceAroundDash_ShouldParse()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(9, 30, 0),
            toTime: new TimeSpan(11, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "forbidden_timerange",
            value: "Monday 09:30-11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Hard);
        result.Severity.Should().Be(ViolationSeverity.Error);
    }
}
