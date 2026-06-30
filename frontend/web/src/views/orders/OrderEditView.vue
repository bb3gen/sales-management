<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'

import OrderForm from '@/components/orders/OrderForm.vue'

import { createEmptyOrder } from '@/types/orders/createEmptyOrder'
import type { ApiErrorResponse } from '@/types/common/apiErrorResponse'
import type { LookupItemDto } from '@/types/common/lookupItemDto'
import type { ProductLookupDto } from '@/types/products/productLookupDto'
import type { UpdateOrderRequest } from '@/types/orders/updateOrderRequest'
import type { ValidationErrors } from '@/types/common/ValidationErrors'

import { getCustomerLookup } from '@/api/customerApi'
import { getProductLookup } from '@/api/productApi'
import { getOrder, updateOrder } from '@/api/orderApi'

const route = useRoute()
const router = useRouter()

const id = route.params.id as string

const form = ref(createEmptyOrder())

const customers = ref<LookupItemDto[]>([])
const products = ref<ProductLookupDto[]>([])
const validationErrors = ref<ValidationErrors>({})
const isSubmitting = ref(false)

const loadLookups = async () => {
  customers.value = await getCustomerLookup()
  products.value = await getProductLookup()
}

const load = async () => {
  try {
    form.value = await getOrder(id)
  } catch (error) {
    console.error(error)

    alert('注文の取得に失敗しました')

    router.push('/orders')
  }
}

const save = async () => {
  validationErrors.value = {}
  isSubmitting.value = true

  const request: UpdateOrderRequest = {
    orderDate: form.value.orderDate || null,
    customerId: form.value.customerId || null,
    details: form.value.details.map((detail) => ({
      productId: detail.productId || null,
      quantity: detail.quantity,
    })),
    updatedAt: form.value.updatedAt,
  }

  try {
    await updateOrder(id, request)

    alert('更新しました')
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

        await load()

        return
      }
    }

    alert('更新に失敗しました')
  } finally {
    isSubmitting.value = false
  }
}

const cancel = () => {
  router.push('/orders')
}

onMounted(async () => {
  await loadLookups()
  await load()
})
</script>

<template>
  <div>
    <h2 class="mb-3">注文編集</h2>

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
