<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import { getMe } from '@/api/authApi'
import router from '@/router'

const authStore = useAuthStore()

const callMe = async () => {
  try {
    const result = await getMe()
    console.log(result)
    alert('成功')
  } catch (e) {
    console.error(e)
    alert('失敗')
  }
}

const logout = async () => {
  await authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div>
    <h1>Home</h1>
    <button @click="callMe">API実行</button>
    <div class="alert alert-success">ログインしています</div>
    <button @click="logout">ログアウト</button>
    <div class="card">
      <div class="card-body">
        <div>Access Token:</div>
        <textarea class="form-control" rows="5" readonly :value="authStore.accessToken" />
      </div>
    </div>
  </div>
</template>
