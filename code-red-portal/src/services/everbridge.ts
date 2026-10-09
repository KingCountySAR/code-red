const API_ROOT = "https://api.everbridge.net";

type Token = {
  access_token: string;
  id_token: string;
  refresh_token: string;
  token_type: string;
  expires_in: number;
  scope: "";
};

class Everbridge {
  async getApiToken(): Promise<Token | null> {
    try {
      const response = await fetch(`${API_ROOT}/authorization/v1/tokens`, {
        method: "POST",
        headers: {
          accept: "application/json",
          "content-type": "application/x-www-form-urlencoded",
        },
        body: new URLSearchParams({
          grant_type: "client_credentials",
          client_id: <string>process.env.EB_CLIENT_ID,
          client_secret: <string>process.env.EB_CLIENT_SECRET,
        }),
      });

      if (!response.ok) {
        throw new Error(`Failed to secure API token`);
      }

      const data = await response.json();

      return data;
    } catch {
      return null;
    }
  }

  async sendViaAPI(token: string, msg: string) {
    const payload = {
      incidentAction: "LaunchThenClose",
      incidentPhases: [
        {
          phaseTemplate: {
            templateId: process.env.EB_TEMPLATE,
            formTemplate: {
              preMessage: msg,
            },
          },
        },
      ],
    };

    try {
      const response = await fetch(
        `${API_ROOT}/rest/incidents/${process.env.EB_ORGANIZATION}`,
        {
          method: "POST",
          headers: {
            accept: "application/json",
            "content-type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(payload),
        }
      );

      if (!response.ok) {
        throw new Error(`Failed to send page!`);
      }
    } catch {}
  }

  async sendMessage(from: string, message: string) {
    const token = (await this.getApiToken()) ?? null;
    if (token && message) {
      await this.sendViaAPI(token.id_token, message);
    } else {
      throw new Error("Token or message empty!");
    }
  }
}

let instance: Everbridge | undefined;

export function getPageSink() {
  let envConfigured = true;

  const required = [
    "EB_CLIENT_ID",
    "EB_CLIENT_SECRET",
    "EB_TEMPLATE",
    "EB_ORGANIZATION",
  ];

  const processEnvKeys = Object.keys(process.env);

  required.forEach((k) => (envConfigured &&= processEnvKeys.includes(k)));

  if (!envConfigured) throw new Error("Process environment misconfigured!");

  instance = new Everbridge();

  if (!instance) throw new Error("Instance creation error!");

  return instance;
}
