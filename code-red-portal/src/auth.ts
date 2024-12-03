import NextAuth from "next-auth";
import Google from "next-auth/providers/google";

export const { handlers, signIn, signOut, auth } = NextAuth({
  providers: [Google],
  callbacks: {
    signIn({ profile }) {
      const allowedDomains = process.env.ALLOWED_DOMAINS?.split(',') ?? [];
      return allowedDomains.some(d => profile?.email?.endsWith(`@${d}`));
    }
  },
  pages: {
    error: "/auth/error",
  }
});