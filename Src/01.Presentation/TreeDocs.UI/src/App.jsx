import React from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { Toaster } from 'react-hot-toast'
import LoginPage from './features/auth/pages/Login'
import RegisterPage from './features/auth/pages/Register'
import DashboardPage from './features/nodes/pages/Dashboard'
import { AuthProvider } from './features/auth/context/AuthContext'

function App() {
  return (
    <Router>
      <AuthProvider>
        <div className="min-h-screen bg-gray-50">
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/" element={<DashboardPage />} />
          </Routes>
        </div>
        <Toaster />
      </AuthProvider>
    </Router>
  )
}

export default App
