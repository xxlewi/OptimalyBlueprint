using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers;

public class BaseController : Controller
{
    protected IActionResult HandleAjaxRequest(string message, bool success = false, object? data = null)
    {
        if (Request.Headers.ContainsKey("X-Requested-With"))
        {
            return Json(new { success, message, data });
        }
        
        // For non-AJAX requests, redirect with message
        if (success)
        {
            return Redirect($"/?success={Uri.EscapeDataString(message)}");
        }
        else
        {
            return Redirect($"/?error={Uri.EscapeDataString(message)}");
        }
    }
    
    protected IActionResult NotImplementedYet(string featureName)
    {
        return HandleAjaxRequest($"Funkce '{featureName}' zatím není implementována. Pracujeme na ní!");
    }
}