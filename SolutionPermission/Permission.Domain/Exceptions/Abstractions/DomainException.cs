namespace Permission.Domain.Exceptions.Abstractions;

public abstract class DomainException : Exception
{
	protected DomainException(string message)
	: base(message){ }

	protected DomainException(string title, string message)
		: base(message) =>
		Title = title;

	public string Title { get; }
}