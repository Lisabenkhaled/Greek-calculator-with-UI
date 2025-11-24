/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.html",
    "./**/*.razor",
    "./**/*.cshtml",
    "./wwwroot/**/*.js",
  ],
  theme: {
    extend: {
      colors: {
        "bg-light": "#f5f7fa",
        "bg-dark": "#0f0f0f",
        "surface-light": "#ffffff",
        "surface-dark": "#1a1a1a",
      },
    },
  },
  plugins: [],
};
