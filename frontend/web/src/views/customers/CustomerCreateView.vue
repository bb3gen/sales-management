<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import type { ValidationErrors } from '@/types/common/ValidationErrors'

import CustomerForm from '@/components/customers/CustomerForm.vue'
import type { CustomerEditDto } from '@/types/customers/customerEditDto'
import type { CreateCustomerRequest } from '@/types/customers/createCustomerRequest'
import { createCustomer } from '@/api/customerApi'

const router = useRouter()
const validationErrors = ref<ValidationErrors>({})

const form = reactive<CustomerEditDto>({
  id: '',
  customerCode: '',
  customerName: '',
  customerKana: '',
  postalCode: '',
  address: '',
  phoneNumber: '',
  email: '',
  updatedAt: '',
})

const save = async () => {
  validationErrors.value = {}

  const request: CreateCustomerRequest = {
    customerName: form.customerName,
    customerKana: form.customerKana,
    postalCode: form.postalCode,
    address: form.address,
    phoneNumber: form.phoneNumber,
    email: form.email,
  }

  try {
    await createCustomer(request)

    alert('登録しました')

    await router.push('/customers')
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 400) {
        validationErrors.value = error.response.data.errors ?? {}
        return
      }
    }

    alert('登録に失敗しました')
  }
}

const back = () => {
  router.push('/customers')
}
</script>

<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2>顧客登録</h2>

      <button class="btn btn-secondary" @click="back">戻る</button>
    </div>

    <div class="card">
      <div class="card-body">
        <CustomerForm
          :is-edit="false"
          v-model:customer-name="form.customerName"
          v-model:customer-kana="form.customerKana"
          v-model:postal-code="form.postalCode"
          v-model:address="form.address"
          v-model:phone-number="form.phoneNumber"
          v-model:email="form.email"
          :validation-errors="validationErrors"
          @submit="save"
        />
      </div>
    </div>
  </div>
</template>
