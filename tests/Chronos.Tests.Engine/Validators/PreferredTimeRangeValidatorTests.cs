using Chronos.Domain.Constraints;
using Chronos.Domain.Schedule;
using Chronos.Engine.Constraints.Evaluation.Validators;
using Chronos.Tests.Engine.TestFixtures;

namespace Chronos.Tests.Engine.Validators;

[TestFixture]
[Category("Unit")]
public class PreferredTimeRangeValidatorTests
{
    private PreferredTimeRangeValidator _validator = null!;
    private ILogger<PreferredTimeRangeValidator> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<PreferredTimeRangeValidator>>();
        _validator = new PreferredTimeRangeValidator(_logger);
    }

    [Test]
    public void ConstraintKey_ShouldBePreferredTimeRange()
    {
        // Assert
        _validator.ConstraintKey.Should().Be("preferred_timerange");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotWithinPreferredRange_ShouldReturnNull()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotExactlyMatchesPreferredRange_ShouldReturnNull()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotStartsAtPreferredStartAndEndsBeforePreferredEnd_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(9, 30, 0),
            toTime: new TimeSpan(10, 30, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotStartsAfterPreferredStartAndEndsAtPreferredEnd_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(10, 0, 0),
            toTime: new TimeSpan(11, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotCompletelyWithinPreferredRange_ShouldReturnNull()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotStartsBeforePreferredRange_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(8, 0, 0),
            toTime: new TimeSpan(9, 30, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ConstraintKey.Should().Be("preferred_timerange");
        result.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
        result.Message.Should().Contain("does not fall within preferred time ranges");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotEndsAfterPreferredRange_ShouldReturnViolation()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
        result.Message.Should().Contain("does not fall within preferred time ranges");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotStartsBeforeAndEndsAfterPreferredRange_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Monday",
            fromTime: new TimeSpan(8, 0, 0),
            toTime: new TimeSpan(12, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
    }

    [Test]
    public async Task ValidateAsync_WhenDifferentWeekday_ShouldReturnViolation()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // When slot is on a different weekday with no preferred ranges for that weekday,
        // validator returns a warning violation
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
        result.Message.Should().Contain("Tuesday");
        result.Message.Should().Contain("does not have any preferred time ranges");
    }

    [Test]
    public async Task ValidateAsync_WhenMultipleCommaSeparatedRanges_ShouldCheckAll()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00, Wednesday 13:00 - 15:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenMultipleNewlineSeparatedRanges_ShouldCheckAll()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Friday",
            fromTime: new TimeSpan(14, 0, 0),
            toTime: new TimeSpan(15, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00\nFriday 14:00 - 16:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotMatchesOneOfMultipleRanges_ShouldReturnNull()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00, Wednesday 13:00 - 15:00, Friday 14:00 - 16:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotDoesNotMatchAnyRangeButWeekdayHasPreferredRanges_ShouldReturnViolation()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00, Wednesday 13:00 - 15:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
        result.Message.Should().Contain("does not fall within preferred time ranges");
        result.Message.Should().Contain("Monday");
    }

    [Test]
    public async Task ValidateAsync_WhenSlotOnWeekdayWithNoPreferredRanges_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot(
            weekday: "Tuesday",
            fromTime: new TimeSpan(9, 0, 0),
            toTime: new TimeSpan(10, 0, 0)
        );
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00, Wednesday 13:00 - 15:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
        result.Message.Should().Contain("does not have any preferred time ranges");
        result.Message.Should().Contain("Tuesday");
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
            key: "preferred_timerange",
            value: "MONDAY 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
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
            key: "preferred_timerange",
            value: "Monday 09:30-11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSpaceAroundDash_ShouldParse()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
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
            key: "preferred_timerange",
            value: ""
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenWhitespaceValue_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        var constraint = TestDataBuilder.CreateConstraint(
            key: "preferred_timerange",
            value: "   "
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
            key: "preferred_timerange",
            value: "invalid format"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Invalid format entries are ignored, so if no valid ranges are found, returns null
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenInvalidTimeFormat_ShouldIgnoreInvalidEntry()
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
            key: "preferred_timerange",
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
            key: "preferred_timerange",
            value: "Monday 11:00 - 09:00" // Invalid: start after end
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Invalid entry is ignored, so if no valid ranges are found, returns null
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenStartTimeEqualsEndTime_ShouldIgnoreInvalidEntry()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 09:30" // Invalid: start equals end
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Invalid entry is ignored, so if no valid ranges are found, returns null
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenNullValue_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource();
        // Null values are handled gracefully by the validator (checked before any processing)
        var constraint = new ActivityConstraint
        {
            Id = Guid.NewGuid(),
            ActivityId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Key = "preferred_timerange",
            Value = null! // Null is handled gracefully, returns null early
        };

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Null values are checked early and return null, not an exception
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenMultipleRangesWithSomeInvalid_ShouldUseValidOnes()
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
            key: "preferred_timerange",
            value: "Monday 11:00 - 09:00, Monday 09:30 - 11:00, invalid format"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Should match the valid range
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenSlotPartiallyOverlapsPreferredRange_ShouldReturnViolation()
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
            key: "preferred_timerange",
            value: "Monday 09:30 - 11:00"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        // Slot must be completely within preferred range, so partial overlap is a violation
        result.Should().NotBeNull();
        result!.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
    }
}
