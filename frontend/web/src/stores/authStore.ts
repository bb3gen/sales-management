import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { apiClient } from '@/api/axios'

export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(localStorage.getItem('accessToken'))
  const refreshToken = ref<string | null>(localStorage.getItem('refreshToken'))

  const isAuthenticated = computed(() => !!accessToken.value)

  const login = (access: string, refresh: string) => {
    accessToken.value = access
    refreshToken.value = refresh

    localStorage.setItem('accessToken', access)
    localStorage.setItem('refreshToken', refresh)
  }

  const logout = async () => {
    try {
      if (refreshToken.value) {
        await apiClient.post('/auth/logout', {
          refreshToken: refreshToken.value,
        })
      }
    } finally {
      accessToken.value = null
      refreshToken.value = null

      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
    }
  }

  return {
    accessToken,
    refreshToken,
    isAuthenticated,
    login,
    logout,
  }
})
