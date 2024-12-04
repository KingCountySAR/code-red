import type { Config } from "tailwindcss";
import daisyui from "daisyui";

export default {
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  daisyui: {
    themes: [{
      nord: {
        // eslint-disable-next-line @typescript-eslint/no-require-imports
        ...require("daisyui/src/theming/themes")["nord"],
        'primary-content': "white",
      },
    },],
  },
  plugins: [daisyui],
} satisfies Config;
