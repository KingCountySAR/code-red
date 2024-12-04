'use server'
import { auth } from "@/auth";
import { redirect } from "next/navigation";

function wait() {
  return new Promise<void>((resolve) => {
    setTimeout(() => resolve(), 1500);
  });
}

export async function sendMessage(formData: FormData): Promise<void> {
  const session = await auth();
  const message = formData.get('message')?.toString();
  console.log(session?.user?.name,'Sending message', message);
  await wait();
  redirect(`/success?message=${encodeURIComponent(message!)}` );
}