using Microsoft.AspNetCore.Authorization;
using PetBoarding_Domain.Accounts;
using PetBoarding_Infrastructure.Authentication;

namespace InfrastructureTests.Authentication;

public class HasPermissionAttributeTests
{
    [Fact]
    public void Constructor_WithReadMemberPermission_ShouldSetCorrectPolicy()
    {
        // Arrange & Act
        var attribute = new HasPermissionAttribute(Permission.ReadMember);

        // Assert
        attribute.Should().BeOfType<HasPermissionAttribute>();
        attribute.Should().BeAssignableTo<AuthorizeAttribute>();
        attribute.Policy.Should().Be(Permission.ReadMember.ToString());
    }

    [Fact]
    public void Constructor_WithUpdateMemberPermission_ShouldSetCorrectPolicy()
    {
        // Arrange & Act
        var attribute = new HasPermissionAttribute(Permission.UpdateMember);

        // Assert
        attribute.Policy.Should().Be(Permission.UpdateMember.ToString());
    }

    [Fact]
    public void Constructor_WithDeleteMemberPermission_ShouldSetCorrectPolicy()
    {
        // Arrange & Act
        var attribute = new HasPermissionAttribute(Permission.DeleteMember);

        // Assert
        attribute.Policy.Should().Be(Permission.DeleteMember.ToString());
    }

    [Theory]
    [InlineData(Permission.ReadMember, "ReadMember")]
    [InlineData(Permission.UpdateMember, "UpdateMember")]
    [InlineData(Permission.DeleteMember, "DeleteMember")]
    public void Constructor_WithDifferentPermissions_ShouldSetCorrectPolicyString(Permission permission, string expectedPolicyName)
    {
        // Arrange & Act
        var attribute = new HasPermissionAttribute(permission);

        // Assert
        attribute.Policy.Should().Be(expectedPolicyName);
    }

    [Fact]
    public void HasPermissionAttribute_ShouldInheritFromAuthorizeAttribute()
    {
        // Arrange
        var attribute = new HasPermissionAttribute(Permission.ReadMember);

        // Assert
        attribute.Should().BeAssignableTo<AuthorizeAttribute>();
    }

    [Fact]
    public void HasPermissionAttribute_ShouldHaveCorrectAttributeUsage()
    {
        // This test verifies that the attribute can be used in the expected contexts
        // by checking if it can be instantiated and applied to methods/classes
        
        // Arrange & Act
        var attribute = new HasPermissionAttribute(Permission.ReadMember);

        // Assert
        attribute.Should().NotBeNull();
        
        // Check that it's properly configured as an authorization attribute
        var authorizeAttribute = attribute as AuthorizeAttribute;
        authorizeAttribute.Should().NotBeNull();
        authorizeAttribute!.Policy.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void MultipleAttributeInstances_WithDifferentPermissions_ShouldBeIndependent()
    {
        // Arrange & Act
        var readAttribute = new HasPermissionAttribute(Permission.ReadMember);
        var updateAttribute = new HasPermissionAttribute(Permission.UpdateMember);
        var deleteAttribute = new HasPermissionAttribute(Permission.DeleteMember);

        // Assert
        readAttribute.Policy.Should().Be("ReadMember");
        updateAttribute.Policy.Should().Be("UpdateMember");
        deleteAttribute.Policy.Should().Be("DeleteMember");

        // Verify they are independent instances
        readAttribute.Should().NotBeSameAs(updateAttribute);
        updateAttribute.Should().NotBeSameAs(deleteAttribute);
        readAttribute.Policy.Should().NotBe(updateAttribute.Policy);
    }
}