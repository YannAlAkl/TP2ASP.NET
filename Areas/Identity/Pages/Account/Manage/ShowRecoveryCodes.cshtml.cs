

#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Areas.Identity.Pages.Account.Manage
{
    
    
    
    
    public class ShowRecoveryCodesModel : PageModel
    {
        
        
        
        
        [TempData]
        public string[] RecoveryCodes { get; set; }

        
        
        
        
        [TempData]
        public string StatusMessage { get; set; }

        
        
        
        
        public IActionResult OnGet()
        {
            if (RecoveryCodes == null || RecoveryCodes.Length == 0)
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }

            return Page();
        }
    }
}
