using Kcsar.Paging.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kcsar.Paging.Web.Controllers
{
  [Authorize(Policy = "UnitMember")]
  public class HomeController : Controller
  {
    private readonly bool armed;
    private readonly CodeRedService codeRed;
    private readonly string groupName;

    public HomeController(IConfiguration config, CodeRedService codeRed)
    {
      bool understood = bool.TryParse(config["disarm"] ?? "false", out bool parsed);
      armed = understood && !parsed;

      this.codeRed = codeRed;
      this.groupName = string.IsNullOrEmpty(config["groupName"]) ? "ESAR" : config["groupName"];
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
      ViewData["groupName"] = groupName;
      base.OnActionExecuted(context);
    }

    [HttpGet]
    public IActionResult Index()
    {
      ViewData["disarmed"] = (!armed).ToString();
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromForm] SendMessageModel model)
    {
      if (ModelState.IsValid)
      {
        if (armed)
        {
          await codeRed.SendMessage($"{User.FindFirst("email")?.Value}", model.Message);
        }

        return RedirectToAction(nameof(Success), new SuccessModel
        {
          When = DateTime.Now.ToString("HH:mm ddd"),
          Message = (armed ? string.Empty : "[disarmed] ") + model.Message
        });
      }

      return View();
    }

    [HttpGet]
    public IActionResult Success([FromQuery] SuccessModel model)
    {
      return View(model);
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
