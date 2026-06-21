<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

const logout = async () => {
  await authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div>
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container-fluid">
        <RouterLink to="/" class="navbar-brand"> 販売管理システム </RouterLink>

        <div class="navbar-nav">
          <RouterLink to="/" class="nav-link"> Home </RouterLink>

          <RouterLink to="/customers" class="nav-link"> 顧客管理 </RouterLink>

          <a href="#" class="nav-link" @click.prevent="logout"> ログアウト </a>
        </div>
      </div>
    </nav>

    <main class="container mt-4">
      <RouterView />
    </main>
  </div>
</template>
