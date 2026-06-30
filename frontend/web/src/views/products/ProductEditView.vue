<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import ProductForm from '@/components/products/ProductForm.vue'
import type { UpdateProductRequest } from '@/types/products/updateProductRequest'
import type { ApiErrorResponse } from '@/types/common/apiErrorResponse'
import type { ValidationErrors } from '@/types/common/ValidationErrors'
import type { ProductEditDto } from '@/types/products/productEditDto'

import { getProduct, updateProduct } from '@/api/productApi'

const route = useRoute()
const router = useRouter()

const id = route.params.id as string
const validationErrors = ref<ValidationErrors>({})

const form = reactive<ProductEditDto>({
  id: '',
  productCode: '',
  productName: '',
  unitPrice: 0,
  unit: '',
  remarks: '',
  updatedAt: '',
})

const load = async () => {
  try {
    const product = await getProduct(id)

    Object.assign(form, product)
  } catch (error) {
    console.error(error)

    alert('商品の取得に失敗しました')

    router.push('/products')
  }
}

const save = async () => {
  validationErrors.value = {}

  const request: UpdateProductRequest = {
    productName: form.productName,
    unitPrice: form.unitPrice,
    unit: form.unit,
    remarks: form.remarks,
    updatedAt: form.updatedAt,
  }

  try {
    await updateProduct(id, request)

    alert('更新しました')

    router.push('/products')
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
  router.push('/products')
}

onMounted(async () => {
  await load()
})
</script>

<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2>商品編集</h2>

      <button class="btn btn-secondary" @click="back">戻る</button>
    </div>

    <div class="card">
      <div class="card-body">
        <ProductForm
          :product-code="form.productCode"
          v-model:productName="form.productName"
          v-model:unitPrice="form.unitPrice"
          v-model:unit="form.unit"
          v-model:remarks="form.remarks"
          :validation-errors="validationErrors"
          @submit="save"
        />
      </div>
    </div>
  </div>
</template>
