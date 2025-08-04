import { create } from 'zustand';

export const useThemeStore = create((set, get) => ({
  isDarkMode: false,
  
  // Initialize theme from localStorage or system preference
  initializeTheme: () => {
    if (typeof window !== 'undefined') {
      const savedTheme = localStorage.getItem('theme');
      if (savedTheme) {
        set({ isDarkMode: savedTheme === 'dark' });
      } else {
        // Check system preference
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        set({ isDarkMode: systemPrefersDark });
      }
      get().applyTheme();
    }
  },
  
  // Toggle between light and dark mode
  toggleTheme: () => {
    const { isDarkMode } = get();
    const newTheme = !isDarkMode;
    set({ isDarkMode: newTheme });
    
    if (typeof window !== 'undefined') {
      localStorage.setItem('theme', newTheme ? 'dark' : 'light');
    }
    
    get().applyTheme();
  },
  
  // Apply theme to document
  applyTheme: () => {
    const { isDarkMode } = get();
    if (typeof document !== 'undefined') {
      if (isDarkMode) {
        document.documentElement.classList.add('dark');
      } else {
        document.documentElement.classList.remove('dark');
      }
    }
  },
  
  // Set specific theme
  setTheme: (theme) => {
    const isDark = theme === 'dark';
    set({ isDarkMode: isDark });
    
    if (typeof window !== 'undefined') {
      localStorage.setItem('theme', isDark ? 'dark' : 'light');
    }
    
    get().applyTheme();
  }
}));
