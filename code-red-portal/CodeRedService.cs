using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kcsar.Paging.Web
{
  public class CodeRedService
  {
    private readonly string subject;
    private readonly string apiKey;
    private readonly string groupName;

    public CodeRedService(IConfiguration config)
    {
      subject = config["codered:subject"];
      apiKey = config["codered:apiKey"];
      groupName = config["codered:groupName"];
    }

    public async Task SendMessage(string from, string text)
    {
      using (var client = new HttpClient() { BaseAddress = new Uri("https://franconia.onsolve.net") })
      {
        var dict = new Dictionary<string, string>();
        dict.Add("api_key", apiKey);
        dict.Add("grant_type", "api_key");
        dict.Add("scope", "onsolve-api");
        dict.Add("client_id", "third-party-api");
        var req = new HttpRequestMessage(HttpMethod.Post, "https://identityservice.onsolve.net/connect/token") { Content = new FormUrlEncodedContent(dict) };
        var res = await client.SendAsync(req);
        res.EnsureSuccessStatusCode();
        JsonElement resJson = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", resJson.GetProperty("access_token").GetString());

        dynamic avoidDevices = (new[] {
          "1-Way Pager",
          "2-Way Pager",
          "Fax",
          "Numeric Pager",
          "Home Phone",
          "Work Phone",
          //"Desktop Alerts",
          "Mobile App",
         // "Personal Mobile Phone 2",
          "TTY Phone",
          "Mobile Phone"
        }).Select(t => new { deviceType = t, priority = "off" }).ToArray();

        string alertJson = JsonSerializer.Serialize(new
        {
          alert = new
          {
            broadcastInfo = new
            {
              recipients = new
              {
                //groups = new[]
                //{
                //  groupName
                //}
                contacts = new[]
                {
                  new { contactId = "4426432e-0001-3000-80c0-fceb55463ffe"}
                }
              }
            },
            locationOverride = new {
              overrideDevices = avoidDevices
            },
            division = "/COUNTY/KCSO/SPECOPS/SAR",
            title = subject,
            confirmResponse = false,
            contactAttemptCycles = 1,
            useAlias = true,
            initiatorAlias = from,
            emailImportance = "Normal",
            verbiage = new
            {
              text = new[]
              {
                new { locale = "en_US", messageType = "Default", value = text }
              }
            }
          }
        });

        Console.WriteLine(alertJson);

        res = await client.PostAsync(
          "https://cascades.onsolve.net/api/v1/Alerts/oneStep",
          new StringContent(alertJson, System.Text.Encoding.UTF8, "application/json")
        );
        res.EnsureSuccessStatusCode();
      }
    }
  }
}
