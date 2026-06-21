<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { ValidationErrors } from '@/types/validation'
import axios from 'axios'

import CustomerForm from '@/components/customers/CustomerForm.vue'
import { getCustomer, updateCustomer } from '@/api/customerApi'

const route = useRoute()
const router = useRouter()

const id = route.params.id as string

const customerCode = ref('')
const customerName = ref('')
const customerKana = ref('')
const postalCode = ref('')
const address = ref('')
const phoneNumber = ref('')
const email = ref('')
const updatedAt = ref('')

const validationErrors = ref<ValidationErrors>({})

onMounted(async () => {
  const customer = await getCustomer(id)
  customerCode.value = customer.customerCode
  customerName.value = customer.customerName
  customerKana.value = customer.customerKana
  postalCode.value = customer.postalCode
  address.value = customer.address
  phoneNumber.value = customer.phoneNumber
  email.value = customer.Email
  updatedAt.value = customer.updatedAt
})

const save = async () => {
  try {
    await updateCustomer(id, {
      customerName: customerName.value,
      customerKana: customerKana.value,
      postalCode: postalCode.value,
      address: address.value,
      phoneNumber: phoneNumber.value,
      email: email.value,
      updatedAt: updatedAt.value,
    })

    await router.push('/customers')
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 409) {
        alert(error.response.data.message)
        return
      }

      if (error.response?.status === 400) {
        validationErrors.value = error.response.data.errors ?? {}

        return
      }
    }

    alert('更新に失敗しました')
  }
}
</script>

<template>
  <div class="container">
    <h2>顧客編集</h2>

    <CustomerForm
      :is-edit="true"
      v-model:customer-code="customerCode"
      v-model:customer-name="customerName"
      v-model:customer-kana="customerKana"
      v-model:postal-code="postalCode"
      v-model:address="address"
      v-model:phone-number="phoneNumber"
      v-model:email="email"
      :validation-errors="validationErrors"
    />

    <button class="btn btn-primary" @click="save">更新</button>
  </div>
</template>
