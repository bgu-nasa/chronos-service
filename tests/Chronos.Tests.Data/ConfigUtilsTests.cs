using Chronos.Data.Utils;

namespace Chronos.Tests.Data;

public class ConfigUtilsTests
{
    [Test]
    public void ToTableName_GivenSingleWord_ReturnsPlural()
    {
        Assert.That(ConfigUtils.ToTableName("User"), Is.EqualTo("users"));
    }
    
    [Test]
    public void ToTableName_GivenMultipleWords_ReturnsPluralWithUnderscore()
    {
        Assert.That(ConfigUtils.ToTableName("UserConfiguration"), Is.EqualTo("user_configurations"));
    }
}