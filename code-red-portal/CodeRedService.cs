using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kcsar.Paging.Web
{
  public class CodeRedService
  {
    private readonly string username;
    private readonly string password;
    private readonly string launchCode;
    private readonly string audience;
    private readonly string subject;
    private readonly string smsPrefix;

    public CodeRedService(IConfiguration config)
    {
      username = config["codered:username"];
      password = config["codered:password"];
      launchCode = config["codered:launch"];
      audience = config["codered:audience"];
      subject = config["codered:subject"];
      smsPrefix = config["codered:smsPrefix"] ?? string.Empty;
    }

    public async Task SendMessage(string fromEmail, string text)
    {
      if (string.IsNullOrWhiteSpace(username) ||
        string.IsNullOrWhiteSpace(password) ||
        string.IsNullOrWhiteSpace(launchCode) ||
        string.IsNullOrWhiteSpace(audience) ||
        string.IsNullOrWhiteSpace(subject))
      {
        throw new ApplicationException("Must define codered:* config - username, password, launch, audience, subject");
      }

      var cookies = await GetAuthCookies();
      using (var handler = new HttpClientHandler() { CookieContainer = cookies })
      {
        using (var client = new HttpClient(handler) { BaseAddress = new Uri("https://next.coderedweb.com/portal/") })
        {
          await SendMessage(client, fromEmail, text, audience, launchCode);
        }
      }
    }

    private async Task SendMessage(HttpClient client, string fromEmail, string theMessage, string audience, string launchCode)
    {
      string scenarioName = $"Scenario {DateTime.Now.ToString("MM/dd/yyyy-HH:mm:ss")}";
      string url = "Scenario/Create?emergency=false&name=" + WebUtility.UrlEncode(scenarioName);
      var scenarioId = await Post(client, url, null, "Couldn't create scenario");

      var tempAudience = await Post(client,
        $"Scenario/UpdateAudience?scenarioID={scenarioId}&audienceID={audience}",
        null,
        "Couldn't create temp audience"
        );

      await Post(client, "Audience/has_tdd_phones", new FormUrlEncodedContent(new Dictionary<string, string> { { "audienceID", tempAudience } }), null);

      string messageName = $"Untitled Message {DateTime.Now.ToString("MM/dd/yyyy-HH:mm:ss")}";
      url = "Message/Create?Visible=false&name=" + WebUtility.UrlEncode(messageName);
      var messageId = await Post(client, url, null, "Couldn't create message");

      await Post(client, "Message/SetName", new FormUrlEncodedContent(new Dictionary<string, string> { { "MessageID", messageId }, { "name", messageName } }), null);

      await Post(client,
        "Message/AddOrUpdateEmailMessage",
        new FormUrlEncodedContent(new Dictionary<string, string>
        {
          { "MessageID", messageId },
          { "Subject", subject },
          { "Sender", fromEmail },
          { "Body", theMessage },
          { "EmailConfirmation", "false" }
        }),
        "Couldn't set email message");

      await Post(client,
        "Message/AddOrUpdateSMSMessage",
        new FormUrlEncodedContent(new Dictionary<string, string>
        {
          { "MessageID", messageId },
          { "SMSSender", "" },
          { "SMSSubject", "" },
          { "SMSBody", smsPrefix + theMessage },
          { "ReplySMSBody", "." }
        }),
        "Couldn't set SMS message"
      );

      await Post(client, "Scenario/UpdateMessage", new FormUrlEncodedContent(new Dictionary<string, string>
      {
        { "scenarioID", scenarioId },
        { "messageID", messageId },
        { "forceTDD", "false" }
      }), "Couldn't update message");

      var launchId = await Post(client,
        $"Scenario/LaunchNow?ScenarioId={scenarioId}&launchCode={launchCode}&autoRecall=true&turbo=undefined",
        new StringContent(""),
        "Couldn't launch");

      await Post(client,
        "BuildScenario/DeleteScenario",
        new FormUrlEncodedContent(new Dictionary<string, string>
        {
          { "scenario_id", scenarioId },
          { "audience_id", "0" },
          { "message_id", messageId }
        }),
        null);

      await Post(client,
        "Audience/Delete",
        new FormUrlEncodedContent(new Dictionary<string, string>
        {
          { "audienceID", tempAudience }
        }),
        null);
    }

    private async Task<string> Post(HttpClient client, string url, HttpContent body, string errorMessage)
    {
      var response = await client.PostAsync(url, body ?? new StringContent(""));
      string responseContent = await response.Content.ReadAsStringAsync();
      var content = JsonConvert.DeserializeObject<JObject>(responseContent);
      if (!string.IsNullOrWhiteSpace(errorMessage))
      {
        ThrowOnError(content, errorMessage);
      }
      return content["Records"]?.Value<string>();
    }

    private void ThrowOnError(JObject content, string message)
    {
      if (content["Result"]?.Value<string>() != "OK")
      {
        throw new ApplicationException(message + "\n" + content.ToString());
      }
    }

    public async Task<CookieContainer> GetAuthCookies()
    {
      var request = (HttpWebRequest)WebRequest.Create("https://next.coderedweb.com/portal/Account/LogOn");
      request.Method = "POST";
      request.AllowAutoRedirect = false;

      string body = JsonConvert.SerializeObject(new
      {
        logonViewModel = new
        {
          UserName = username,
          Password = password,
          RememberMe = false
        },
        returnUrl = (string)null
      });
      byte[] byte1 = Encoding.UTF8.GetBytes(body);

      // Set the content type of the data being posted.
      request.ContentType = "application/json";

      // Set the content length of the string being posted.
      request.ContentLength = byte1.Length;

      Stream newStream = request.GetRequestStream();

      newStream.Write(byte1, 0, byte1.Length);

      var loginResponse = (HttpWebResponse)(await request.GetResponseAsync());
      using (var reader = new StreamReader(loginResponse.GetResponseStream()))
      {
        var text = await reader.ReadToEndAsync();
      }
      var cCount = loginResponse.Cookies.Count;
      var h = loginResponse.GetResponseHeader("Set-Cookie");

      CookieContainer cc = new CookieContainer();
      cc.SetCookies(new Uri("https://next.coderedweb.com/portal/Account/Login"), h);
      var cookies = cc.GetCookieHeader(new Uri("https://next.coderedweb.com/portal/test"));

      return cc;
    }

  }
}
