# .NET Unit Test Best Practices

## General Principles
- Use the AAA pattern: **Arrange**, **Act**, **Assert** for all tests.
- Name tests clearly and descriptively: `[MethodName_StateUnderTest_ExpectedBehavior]`.
- Group related tests in classes named `[ClassName]Tests`.
- No use AAA pattern comments; structure should be clear enough.

## Isolation & Mocking
- Isolate units under test; mock dependencies using libraries like NSubstitute or Moq.
- Avoid reliance on external systems (database, file system, network).
- Use in-memory providers for EF Core when needed.

## Assertions
- Use FluentAssertions for expressive, readable assertions.
- Assert only one logical outcome per test.
- Prefer specific assertions over generic ones.

## Setup & Teardown
- Use `IClassFixture` or constructor for setup.
- Clean up resources after tests; avoid side effects.

## Coverage & Edge Cases
- Aim for high coverage, focusing on business logic and edge cases.
- Test exception paths and invalid inputs.

## Performance
- Keep tests fast; avoid long-running operations.
- Use `[Fact]` for simple tests, `[Theory]` for parameterized tests.

## Maintainability
- Refactor tests when production code changes.
- Avoid duplication; use helper methods for common setup.
- Keep test code as clean as production code.

## CI/CD Integration
- Ensure tests run in CI/CD pipelines; fail builds on test failures.

## Documentation
- Comment complex test logic and document intent if not obvious.

## Example

```csharp
[Fact]
public void CalculateTotal_WithValidItems_ReturnsCorrectSum()
{
	// Arrange
	var cart = new ShoppingCart();
	cart.AddItem(new Item("Book", 10));
	cart.AddItem(new Item("Pen", 2));

	// Act
	var total = cart.CalculateTotal();

	// Assert
	total.Should().Be(12);
}
```
