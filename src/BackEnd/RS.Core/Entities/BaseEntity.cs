using System.ComponentModel.DataAnnotations;

namespace RS.Core.Entities;

public class BaseEntity
{
	[Required]
	public ulong Id { get; private set; }

	[Required]
	public DateTime CreatedAt { get; private set; } = DateTime.Now;
}
