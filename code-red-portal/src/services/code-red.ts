const AVOID_DEVICES = [
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
  "Mobile Phone",
];

class CodeRed {
  async sendMessage(from: string, message: string) {
    const authResponse = await fetch("https://identityservice.onsolve.net/connect/token", {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      },
      body: new URLSearchParams({
        'api_key': process.env.CODERED_APIKEY!,
        'grant_type': 'api_key',
        'scope': 'onsolve-api',
        "client_id": "third-party-api",
      })
    });

    if (authResponse.status !== 200) {
      throw Error("Failed to get auth token");
    }

    const token = (await authResponse.json()).access_token;
    let recipients: {groups: string[]}|{contacts:{contactId: string}[]} = { groups: [ process.env.CODERED_GROUP ?? '' ]};
    if (process.env.CODERED_CONTACT) {
      console.log('Sending to test contact', process.env.CODERED_CONTACT);
      recipients = ({ contacts: [ { contactId: process.env.CODERED_CONTACT }]});
    }

    const alertData = {
      alert: {
        broadcastInfo: {
          recipients,
        },
        locationOverride: {
          overrideDevices: AVOID_DEVICES.map(t => ({ deviceType: t, priority: "off"})),
        },
        divison: "/COUNTY/KCSO/SPECOPS/SAR",
        title: process.env.CODERED_SUBJECT ?? 'No subject',
        confirmResponse: false,
        contactAttemptCycles: 1,
        useAlias: true,
        initiatorAlias: from,
        emailImportance: "Normal",
        verbiage: {
          text: [
            { locale: "en_US", messageType: "Default", value: message }
          ]
        }
      }
    };

    const res = await fetch("https://cascades.onsolve.net/api/v1/Alerts/oneStep", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`,
      },
      body: JSON.stringify(alertData)
    });
    const success = res.status >= 200 && res.status < 400;
    if (!success) {
      console.log(await res.text());
      throw Error('unable to send message');
    }
  }
}

let instance: CodeRed | undefined;
export function getCodeRed() {
  if (!instance) {
    if (process.env.CODERED_APIKEY && process.env.CODERED_SUBJECT && (process.env.CODERED_GROUP || process.env.CODERED_CONTACT)) {
      console.log('Building CodeRed instance');
      instance = new CodeRed();
    } else {
      throw "Must supply CODERED_APIKEY, CODERED_SUBJECT, and CODERED_GROUP or CODERED_CONTACT";
    }
  }
  return instance;
}