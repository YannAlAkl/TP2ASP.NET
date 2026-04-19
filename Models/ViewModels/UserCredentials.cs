namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models.ViewModels
{
	public class UserCredentials
	{
		public int Id { get; set; }
		public string UserName { get; set; }= string.Empty;
		public string Email { get; set; }= string.Empty;
		public string Password { get; set; }= string.Empty;

		public string ConfirmPassword { get; set; } = string.Empty;

		public string Status { get; set; }= string.Empty;


	}
}
