import { auth } from "@/auth";
import SignIn from "@/components/sign-in";
import TitleBar from "@/components/title-bar";

export default async function AuthedLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const session = (await auth())!;

  if (session?.user) {
    return (
      <div className="flex flex-col min-h-screen">
        <TitleBar team={process.env.TEAM_NAME ?? 'Your Team'} name={session.user.name} />
        {children}
      </div>
    );
  }

  return (
    <div className="grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20 font-[family-name:var(--font-geist-sans)]">
      <SignIn />
    </div>
  );
}
