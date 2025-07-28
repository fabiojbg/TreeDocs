import axios from 'axios'

const API_BASE_URL = '/api'

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
    const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
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
    if (error.response?.status === 401) {
      if (logoutCallback) {
        logoutCallback();
      } else {
        // Fallback if callback not set (shouldn't happen in production)
        localStorage.removeItem('authToken')
        localStorage.removeItem('userData')
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

export default api
