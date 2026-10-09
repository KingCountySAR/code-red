'use server'
import { auth } from "@/auth";
import { getPageSink } from "@/services/everbridge";
import { redirect } from "next/navigation";

export async function sendMessage(formData: FormData): Promise<void> {
  const session = await auth();
  if (!session?.user?.email) {
    throw Error("Can't get email for user");
  }

  const message = formData.get('message')?.toString();
  if (!message) {
    throw Error("No message");
  }

  await getPageSink().sendMessage(session.user.email, message);
  const responseMessage = `${process.env.CODERED_DISARM ? '[disarmed] ' : ''}${message}`;
  redirect(`/success?message=${encodeURIComponent(responseMessage)}` );
}