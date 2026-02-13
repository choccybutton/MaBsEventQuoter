import axios, { AxiosInstance, AxiosError } from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

/**
 * API client instance with default configuration.
 */
const client: AxiosInstance = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
})

/**
 * Add request interceptor for auth tokens (when implemented).
 */
client.interceptors.request.use(
  (config) => {
    // Add authorization token here if available
    // const token = localStorage.getItem('authToken')
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`
    // }
    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

/**
 * Add response interceptor for error handling.
 */
client.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      // Handle unauthorized - redirect to login
      // window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

export default client
