<script setup lang="ts">
import { ref } from 'vue'
import { sendOtp, verifyOtp } from '@/api/authApi'

import { useAuthStore } from '@/stores/authStore'
import router from '@/router'

const authStore = useAuthStore()

const email = ref('')
const code = ref('')

const otpSent = ref(false)

const send = async () => {
  await sendOtp(email.value)

  otpSent.value = true
}

const verify = async () => {
  const result = await verifyOtp(email.value, code.value)

  authStore.login(result.accessToken, result.refreshToken)

  await router.push('/')
}
</script>

<template>
  <div class="container mt-5">
    <h2>Login</h2>

    <input v-model="email" class="form-control mb-2" placeholder="Email" />

    <button class="btn btn-primary" @click="send">Send OTP</button>

    <div v-if="otpSent">
      <input v-model="code" class="form-control mt-3" placeholder="OTP" />

      <button class="btn btn-success mt-2" @click="verify">Verify</button>
    </div>
  </div>
</template>
