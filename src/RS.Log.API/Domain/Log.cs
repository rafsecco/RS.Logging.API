using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS.Log.API.Domain
{
	public class Log
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; private set; }

		[Required]
		[Column(TypeName = "VARCHAR(100)")]
		[MaxLength(100)]
		public string TenantId { get; set; }

		public DateTime DateCreated { get; private set; } = DateTime.Now;

		[Column(TypeName = "VARCHAR(255)")]
		public string Message { get; set; }

		public string StackTrace { get; set; }

	}
}
