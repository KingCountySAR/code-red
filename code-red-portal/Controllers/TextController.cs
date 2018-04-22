using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace Kcsar.Paging.Web.Controllers
{
  public class TextController : TwilioController
  {
    private readonly CodeRedService codeRed;
    private readonly ILogger<TextController> log;

    Dictionary<string, string> whitelist;

    public TextController(CodeRedService codeRed, IConfiguration config, ILogger<TextController> log)
    {
      this.codeRed = codeRed;
      this.log = log;

      whitelist = (config["smsWhitelist"] ?? "").Split(',').Select(f => f.Split(':')).Where(f => f.Length == 2).ToDictionary(f => f[0], f => f[1]);
    }

    // GET: Incoming messages
    [HttpPost]
    public async Task<TwiMLResult> Index([FromForm] SmsRequest request)
    {
      var response = new MessagingResponse();

      if (whitelist.TryGetValue(request.From, out string email))
      {
        log.LogTrace($"Forwarding message from {request.From}/{email}: {request.Body}");
        await codeRed.SendMessage(email, request.Body);
      }
      else
      {
        log.LogWarning($"Rejected message from {request.From}: {request.Body}");
        response.Message("Unknown number. Page not sent.");
      }

      return TwiML(response);
    }
  }
}
