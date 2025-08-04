/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      spacing: {
        '1': '0.15rem', // Redefining py-1 from default 0.25rem to 0.5rem
      },
    },
  },
  plugins: [],
}
