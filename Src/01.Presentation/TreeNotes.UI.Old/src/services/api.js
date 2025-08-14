import axios from 'axios'
import toast from 'react-hot-toast'

const API_BASE_URL = (window._env_.TREE_NOTES_SERVICE_URL !== "REPLACE_TREE_NOTES_SERVICE_URL" ?  window._env_.TREE_NOTES_SERVICE_URL 
                                                                                               : "http://localhost:3000") 
                    + "/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

let logoutCallback = null;

export const setLogoutCallback = (callback) => {
  logoutCallback = callback;
};

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

function convertKeysToCamelCase(obj) {
  if (obj === null || typeof obj !== 'object') {
    return obj;
  }

  if (Array.isArray(obj)) {
    return obj.map(item => convertKeysToCamelCase(item));
  }

  return Object.keys(obj).reduce((acc, key) => {
    const camelKey = key.startsWith('_')
      ? '_' + key.charAt(1).toLowerCase() + key.slice(2)
      : key.charAt(0).toLowerCase() + key.slice(1);
    acc[camelKey] = convertKeysToCamelCase(obj[key]);
    return acc;
  }, {});
}

// Response interceptor to handle auth errors and normalize API response keys
api.interceptors.response.use(
  (response) => {
    response.data = convertKeysToCamelCase(response.data);
    return response;
  },
  (error) => {
    let message = 'An unexpected error occurred.';
    if (error.response?.data) {
      error.response.data = convertKeysToCamelCase(error.response.data);
      // Check for '_Message' (converted to '_message' by camelCase converter) first
      message = error.response.data._message || 
                error.response.data.message || 
                message;
    } else {
      message = error.message;
    }
    
    toast.error(message); // Display error toast
    
    if (error.response?.status === 401) {
      if (logoutCallback) {
        logoutCallback();
      } else {
        localStorage.removeItem('authToken')
        localStorage.removeItem('userData')
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

export default api
