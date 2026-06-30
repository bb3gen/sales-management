<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

import ProductForm from '@/components/products/ProductForm.vue'
import { createProduct } from '@/api/productApi'
import type { ProductEditDto } from '@/types/products/productEditDto'
import type { ValidationErrors } from '@/types/common/ValidationErrors'
import type { CreateProductRequest } from '@/types/products/createProductRequest'

const router = useRouter()
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

const save = async () => {
  validationErrors.value = {}

  const request: CreateProductRequest = {
    productName: form.productName,
    unitPrice: form.unitPrice,
    remarks: form.remarks,
  }

  try {
    await createProduct(request)

    alert('登録しました')

    await router.push('/products')
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
  router.push('/products')
}
</script>

<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2>商品登録</h2>

      <button class="btn btn-secondary" @click="back">戻る</button>
    </div>

    <div class="card">
      <div class="card-body">
        <ProductForm
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
