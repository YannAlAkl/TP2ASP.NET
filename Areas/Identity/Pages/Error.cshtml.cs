

#nullable disable

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Areas.Identity.Pages
{
    
    
    
    
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        
        
        
        
        public string RequestId { get; set; }

        
        
        
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        
        
        
        
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
