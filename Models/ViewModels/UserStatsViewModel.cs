using System;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models.ViewModels
{
	public class UserStatsViewModel
	{
		public int Id { get; set; }
		public  string UserName { get; set; } = string.Empty;
		public int SubjectCount { get; set; }
		public int MessageCount { get; set; }

		public DateTime  LastActivity { get; set; }
		public string? Email { get; internal set; }
	}
}
