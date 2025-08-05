import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './auth/context/AuthContext';
import { ClipLoader } from 'react-spinners';

const ProtectedRoute = ({ element }) => {
  const { user, loading } = useAuth();

  if (loading) {
    // Optionally, render a loading spinner while checking auth status
    return (
      <div className="flex items-center justify-center min-h-screen">
        <ClipLoader color="#3b82f6" loading={loading} size={50} />
      </div>
    );
  }

  return user ? element : <Navigate to="/login" replace />;
};

export default ProtectedRoute;
