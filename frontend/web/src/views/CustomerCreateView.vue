<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { createCustomer } from '@/api/customerApi'
import type { ValidationErrors } from '@/types/validation'
import CustomerForm from '@/components/customers/CustomerForm.vue'

const router = useRouter()

const customerName = ref('')
const customerKana = ref('')
const postalCode = ref('')
const address = ref('')
const phoneNumber = ref('')
const email = ref('')

const validationErrors = ref<ValidationErrors>({})

const save = async () => {
  try {
    validationErrors.value = {}

    await createCustomer({
      customerName: customerName.value,
      customerKana: customerKana.value,
      postalCode: postalCode.value,
      address: address.value,
      phoneNumber: phoneNumber.value,
      email: email.value,
    })

    //await router.push('/customers')
  } catch (error: any) {
    if (error.response?.status === 400) {
      validationErrors.value = error.response.data.errors ?? {}

      return
    }

    alert('登録に失敗しました')
  }
}
</script>

<template>
  <div class="container">
    <h2 class="mb-4">顧客登録</h2>

    <CustomerForm
      :is-edit="false"
      v-model:customer-name="customerName"
      v-model:customer-kana="customerKana"
      v-model:postal-code="postalCode"
      v-model:address="address"
      v-model:phone-number="phoneNumber"
      v-model:email="email"
      :validation-errors="validationErrors"
    />

    <button class="btn btn-primary" @click="save">登録</button>
  </div>
</template>
