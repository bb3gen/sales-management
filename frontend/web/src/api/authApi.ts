import { apiClient } from './axios'

export const sendOtp = async (email: string) => {
  const response = await apiClient.post('/auth/send-otp', {
    email,
  })

  return response.data
}

export const verifyOtp = async (email: string, code: string) => {
  const response = await apiClient.post('/auth/verify-otp', {
    email,
    code,
  })

  return response.data
}

export const refreshToken = async (refreshToken: string) => {
  const response = await apiClient.post('/auth/refresh', {
    refreshToken,
  })

  return response.data
}

export async function logout() {
  const refreshToken = localStorage.getItem('refreshToken')

  if (refreshToken) {
    await apiClient.post('/auth/logout', {
      refreshToken,
    })
  }
}

// テスト
export async function getMe() {
  const response = await apiClient.get('/me')

  return response.data
}
