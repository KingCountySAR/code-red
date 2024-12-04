import { auth } from "@/auth";
import { sendMessage } from "./actions";
import OneShotSubmit from "@/components/one-shot-submit";

export default async function Home() {
  const session = (await auth())!;
  if (!session?.user) return null;

  return (
    <div className="flex-auto grid grid-rows-[20px_1fr_20px] justify-items-stretch p-8 gap-16 sm:p-16 font-[family-name:var(--font-geist-sans)]">
      <main className="flex flex-col gap-8 items-stretch">
        <form action={sendMessage}>
          <div className="flex flex-col">
            <textarea name="message" className="flex textarea textarea-bordered" placeholder="Message Content" required />
          </div>
          <div className="flex justify-end py-3">
            <OneShotSubmit text="Send" />
          </div>
        </form>
      </main>
      <footer className="row-start-3 flex gap-6 flex-wrap items-center justify-center">
      </footer>
    </div>
  );
}
