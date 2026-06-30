<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import type { ValidationErrors } from '@/types/common/ValidationErrors'
import type { ApiErrorResponse } from '@/types/common/apiErrorResponse'

import CustomerForm from '@/components/customers/CustomerForm.vue'
import type { CustomerEditDto } from '@/types/customers/customerEditDto'
import type { UpdateCustomerRequest } from '@/types/customers/updateCustomerRequest'
import { getCustomer, updateCustomer } from '@/api/customerApi'

const route = useRoute()
const router = useRouter()

const id = route.params.id as string
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

const load = async () => {
  try {
    const customer = await getCustomer(id)

    Object.assign(form, customer)
  } catch (error) {
    console.error(error)

    alert('顧客の取得に失敗しました')

    router.push('/customers')
  }
}

const save = async () => {
  validationErrors.value = {}

  const request: UpdateCustomerRequest = {
    customerName: form.customerName,
    customerKana: form.customerKana,
    postalCode: form.postalCode,
    address: form.address,
    phoneNumber: form.phoneNumber,
    email: form.email,
    updatedAt: form.updatedAt,
  }

  try {
    await updateCustomer(id, request)

    alert('更新しました')

    await router.push('/customers')
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 400) {
        validationErrors.value = error.response.data.errors ?? {}
        return
      }

      if (error.response?.status === 409) {
        const response = error.response.data as ApiErrorResponse
        alert(response.message)

        await load()

        return
      }
    }

    alert('更新に失敗しました')
  }
}

const back = () => {
  router.push('/customers')
}

onMounted(async () => {
  await load()
})
</script>

<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2>顧客編集</h2>
      <button class="btn btn-secondary" @click="back">戻る</button>
    </div>

    <div class="card">
      <div class="card-body">
        <CustomerForm
          v-model:customer-code="form.customerCode"
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
