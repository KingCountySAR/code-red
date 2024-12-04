import MessagingResponse from "twilio/lib/twiml/MessagingResponse";
import { getCodeRed } from "@/services/code-red";

export async function POST(request: Request) {
  const whitelist = (process.env.SMS_WHITELIST ?? '')
    .split(',')
    .map(f => f.split(':'))
    .filter(f => f.length == 2)
    .reduce((a,c) => ({ ...a, [c[0]]: c[1] }), {} as Record<string,string>);

  const formData = await request.formData()
  const from = formData.get('From')?.toString() ?? "not-found";
  const body = formData.get('Body')?.toString() ?? "empty body";

  const response = new MessagingResponse();

  if (whitelist[from]) {
    response.message('Got it');
    console.log(`Going to send message from ${whitelist[from]}: ${body}`);
    getCodeRed().sendMessage(whitelist[from], body);
  } else {
    response.message('Not in the list of known numbers. Rejected.');
  }

  return new Response(response.toString(), { headers: { "Content-Type": "text/xml" } })
}