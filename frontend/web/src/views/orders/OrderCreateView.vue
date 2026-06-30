<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

import OrderForm from '@/components/orders/OrderForm.vue'

import { createEmptyOrder } from '@/types/orders/createEmptyOrder'
import type { LookupItemDto } from '@/types/common/lookupItemDto'
import type { ProductLookupDto } from '@/types/products/productLookupDto'
import type { ValidationErrors } from '@/types/common/ValidationErrors'
import type { CreateOrderRequest } from '@/types/orders/createOrderRequest'
import type { ApiErrorResponse } from '@/types/common/apiErrorResponse'

import { getCustomerLookup } from '@/api/customerApi'
import { getProductLookup } from '@/api/productApi'
import { createOrder } from '@/api/orderApi'

const router = useRouter()

const form = ref(createEmptyOrder())

const customers = ref<LookupItemDto[]>([])
const products = ref<ProductLookupDto[]>([])
const validationErrors = ref<ValidationErrors>({})
const isSubmitting = ref(false)

const loadLookups = async () => {
  customers.value = await getCustomerLookup()
  products.value = await getProductLookup()
}

const save = async () => {
  validationErrors.value = {}
  isSubmitting.value = true

  const request: CreateOrderRequest = {
    orderDate: form.value.orderDate || null,
    customerId: form.value.customerId || null,
    details: form.value.details.map((detail) => ({
      productId: detail.productId || null,
      quantity: detail.quantity,
    })),
  }

  try {
    await createOrder(request)

    alert('登録しました')
    router.push('/orders')
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 400) {
        validationErrors.value = error.response.data.errors ?? {}
        return
      }

      if (error.response?.status === 409) {
        const response = error.response.data as ApiErrorResponse
        alert(response.message)
        return
      }
    }

    alert('登録に失敗しました')
  } finally {
    isSubmitting.value = false
  }
}

const cancel = () => {
  router.push('/orders')
}

onMounted(loadLookups)
</script>

<template>
  <div>
    <h2 class="mb-3">受注登録</h2>

    <OrderForm
      v-model:form="form"
      :customers="customers"
      :products="products"
      :errors="validationErrors"
      :is-submitting="isSubmitting"
      @submit="save"
      @cancel="cancel"
    />
  </div>
</template>
