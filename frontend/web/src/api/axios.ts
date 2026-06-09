import axios from 'axios'
import { useAuthStore } from '@/stores/authStore'

export const apiClient = axios.create({
  baseURL: 'http://localhost:5103/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

// リクエスト
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

// レスポンス
apiClient.interceptors.response.use(
  (response) => response,

  async (error) => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      const authStore = useAuthStore()

      try {
        const response = await axios.post('http://localhost:5103/api/auth/refresh', {
          refreshToken: authStore.refreshToken,
        })

        authStore.login(response.data.accessToken, response.data.refreshToken)

        originalRequest.headers.Authorization = `Bearer ${response.data.accessToken}`

        return apiClient(originalRequest)
      } catch {
        authStore.logout()

        window.location.href = '/login'

        return Promise.reject(error)
      }
    }

    return Promise.reject(error)
  },
)
