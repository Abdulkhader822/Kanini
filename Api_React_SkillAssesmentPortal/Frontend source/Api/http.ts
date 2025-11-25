import axios from "axios";
import { toast } from 'react-toastify';
import { tokenstore } from "../Auth/tokenstore";

// Example: VITE_API_BASE_URL = https://localhost:7186/api
export const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "/api",
  headers: { "Content-Type": "application/json" },
});

// Request Interceptor
http.interceptors.request.use(
  (config) => {
    const token = tokenstore.get();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor
http.interceptors.response.use(
  (response) => {
    // Let components handle their own success messages
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      tokenstore.clear();
      window.location.href = "/login";
    } else if (error.response) {
      const url = error.config?.url || '';
      const isAuthEndpoint = url.includes('/Auth/login') || url.includes('/Auth/register');
      
      // Let auth components handle their own errors
      if (!isAuthEndpoint) {
        const specificError = error.response.data?.error || error.response.data?.message;
        
        if (specificError) {
          toast.error(specificError);
        } else if (error.response.status === 400) {
          toast.error("Invalid request. Please check your input and try again.");
        } else {
          toast.error(`API Error: ${error.response.statusText || 'An error occurred.'}`);
        }
      }
    } else if (error.message === 'Network Error' || error.code === 'ERR_NETWORK') {
      toast.error("Network Error: Could not reach the server. Please check your connection.");
    } else {
      toast.error("An unexpected application error occurred. Please try again.");
    }
    return Promise.reject(error);
  }
);
