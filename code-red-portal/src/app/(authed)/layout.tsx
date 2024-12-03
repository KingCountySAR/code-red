import { auth } from "@/auth";
import SignIn from "@/components/sign-in";

export default async function AuthedLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const session = (await auth())!;

  if (session?.user) {
    return children;
  }

  return (
    <div className="grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20 font-[family-name:var(--font-geist-sans)]">
      <SignIn />
    </div>
  );
}
