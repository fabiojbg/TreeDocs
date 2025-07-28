import React, { createContext, useState, useContext, useEffect } from 'react'
import { setLogoutCallback } from '../services/api'; // Import the setLogoutCallback

const AuthContext = createContext()

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Check for stored auth token on app load
    const token = localStorage.getItem('authToken')
    const userData = localStorage.getItem('userData')
    
    if (token && userData) {
      try {
        const parsedUser = JSON.parse(userData)
        setUser(parsedUser)
      } catch (error) {
        // Invalid stored data, clear it
        localStorage.removeItem('authToken')
        localStorage.removeItem('userData')
      }
    }
    setLoading(false)
  }, [])

  const login = (userData) => {
    // Normalize token property to ensure consistent naming (token instead of Token)
    const normalizedUserData = {
      ...userData,
      token: userData.token || userData.Token
    };
    setUser(normalizedUserData)
    localStorage.setItem('authToken', normalizedUserData.token)
    localStorage.setItem('userData', JSON.stringify(normalizedUserData))
  }

  const logout = () => {
    setUser(null)
    localStorage.removeItem('authToken')
    localStorage.removeItem('userData')
  }

  // Set the logout callback for the API interceptor
  useEffect(() => {
    setLogoutCallback(logout);
  }, [logout]); // Re-run if logout function changes (though it's stable here)

  const value = {
    user,
    login,
    logout,
    loading
  }

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}
