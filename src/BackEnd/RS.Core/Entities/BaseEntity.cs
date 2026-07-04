using System.ComponentModel.DataAnnotations;

namespace RS.Core.Entities;

public class BaseEntity
{
	[Required]
	public long Id { get; private set; }

	[Required]
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
