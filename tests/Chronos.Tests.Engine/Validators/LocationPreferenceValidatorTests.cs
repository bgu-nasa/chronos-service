using Chronos.Domain.Constraints;
using Chronos.Engine.Constraints.Evaluation.Validators;
using Chronos.Tests.Engine.TestFixtures;

namespace Chronos.Tests.Engine.Validators;

[TestFixture]
[Category("Unit")]
public class LocationPreferenceValidatorTests
{
    private LocationPreferenceValidator _validator = null!;
    private ILogger<LocationPreferenceValidator> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<LocationPreferenceValidator>>();
        _validator = new LocationPreferenceValidator(_logger);
    }

    [Test]
    public void ConstraintKey_ShouldBeLocationPreference()
    {
        // Assert
        _validator.ConstraintKey.Should().Be("location_preference");
    }

    [Test]
    public async Task ValidateAsync_WhenLocationMatches_ShouldReturnNull()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(location: "Building A");
        var constraint = TestDataBuilder.CreateConstraint(
            key: "location_preference",
            value: "Building A,Building B,Building C"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WhenLocationDoesNotMatch_ShouldReturnViolation()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(location: "Building D");
        var constraint = TestDataBuilder.CreateConstraint(
            key: "location_preference",
            value: "Building A,Building B,Building C"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().NotBeNull();
        result!.ConstraintKey.Should().Be("location_preference");
        result.ViolationType.Should().Be(ViolationType.Soft);
        result.Severity.Should().Be(ViolationSeverity.Warning);
        result.Message.Should().Contain("Building D");
        result.Message.Should().Contain("not in preferred locations");
    }

    [Test]
    public async Task ValidateAsync_CaseInsensitive_ShouldMatch()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(location: "building a");
        var constraint = TestDataBuilder.CreateConstraint(
            key: "location_preference",
            value: "BUILDING A,BUILDING B"
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
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(location: "Building B");
        var constraint = TestDataBuilder.CreateConstraint(
            key: "location_preference",
            value: " Building A , Building B , Building C "
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
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(location: "Building A");
        var constraint = TestDataBuilder.CreateConstraint(key: "location_preference", value: "");

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_SingleLocation_ShouldWork()
    {
        // Arrange
        var activity = TestDataBuilder.CreateActivity();
        var slot = TestDataBuilder.CreateSlot();
        var resource = TestDataBuilder.CreateResource(location: "Main Campus");
        var constraint = TestDataBuilder.CreateConstraint(
            key: "location_preference",
            value: "Main Campus"
        );

        // Act
        var result = await _validator.ValidateAsync(constraint, activity, slot, resource);

        // Assert
        result.Should().BeNull();
    }
}
