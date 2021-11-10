using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS.Log.Domain
{
	[Table("Logs")]
	public class Log
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; private set; }

		[Required]
		public DateTime DateCreated { get; private set; } = DateTime.Now;

		[Required]
		[Column(TypeName = "VARCHAR(255)")]
		public string Message { get; set; }

		[Column(TypeName = "VARCHAR(255)")]
		public string StackTrace { get; set; }
	}
}
