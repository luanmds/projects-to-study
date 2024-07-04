using Application.Exceptions;
using AutoFixture.Xunit2;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace EncryptSecretProject.UnitTests.Infrastructure.Repositories;

[Trait("Layer", "Infrastructure")]
[Trait("Component", "SecretRepository")]
public class SecretRepositoryTests
{
    private readonly DbSet<Secret> _mockDbSet = Substitute.For<DbSet<Secret>, IQueryable<Secret>>();
    private readonly SecretDbContext _context = Substitute.For<SecretDbContext>();
    private readonly SecretRepository _secretRepository;

    public SecretRepositoryTests()
    {
        _context.Secrets.Returns(_mockDbSet);
        _secretRepository = new SecretRepository(_context);
    }

    [Theory(DisplayName = "Given Secret instance, should call AddAsync in context")]
    [AutoData]
    public async Task AddAsync__GivenSecretInstance__ShouldCallAddAsyncInContext(Secret secret)
    {
        // Act
        await _secretRepository.AddAsync(secret);
        
        // Assert
        await _mockDbSet.Received().AddAsync(secret, Arg.Any<CancellationToken>());
    }
    
    [Theory(DisplayName = "Given valid secret id, should returns Secret")]
    [AutoData]
    public async Task GetById__GivenValidSecretId__ShouldReturnSecret(string secretId, Secret expectedSecret)
    {
        // Arrange
        _mockDbSet.FindAsync(secretId).Returns(expectedSecret);
        
        // Act
        var secret = await _secretRepository.GetById(secretId);
        
        // Assert
        await _mockDbSet.Received().FindAsync(secretId);
        Assert.Equivalent(secret, expectedSecret);
    }
    
    [Theory(DisplayName = "Given invalid secret id, should throws AggregateNotFoundException")]
    [AutoData]
    public async Task GetById__GivenInvalidSecretId__ShouldThrowAggregateNotFoundException(string secretId)
    {
        // Arrange
        _mockDbSet.FindAsync(secretId).ReturnsNull();
        
        // Act
        var sut = async () =>  await _secretRepository.GetById(secretId);
        
        // Assert
        await Assert.ThrowsAsync<AggregateNotFoundException>(sut);
        await _mockDbSet.Received().FindAsync(secretId);
    }
    
    [Theory(DisplayName = "Given Secret instance, should call Update in context")]
    [AutoData]
    public void Update__GivenSecretInstance__ShouldCallUpdateInContext(Secret secret)
    {
        // Act
        _secretRepository.Update(secret);
        
        // Assert
        _mockDbSet.Received().Update(secret);
    }
    
    [Fact(DisplayName = "When commit method calls, should save changes")]
    public async Task Commit__WhenCommitMethodCalls__ShouldSaveChanges()
    {
        // Act
        await _secretRepository.Commit();
        
        // Assert
        await _context.Received().SaveChangesAsync();
    }
}