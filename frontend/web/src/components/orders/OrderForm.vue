<script setup lang="ts">
import OrderDetailGrid from '@/components/orders/OrderDetailGrid.vue'
import type { OrderEditDto } from '@/types/orders/OrderEditDto'
import type { ProductLookupDto } from '@/types/products/productLookupDto'
import type { LookupItemDto } from '@/types/common/lookupItemDto'
import type { ValidationErrors } from '@/types/common/ValidationErrors'

interface Props {
  customers: LookupItemDto[]
  products: ProductLookupDto[]
  errors: ValidationErrors
  isSubmitting?: boolean
}

defineProps<Props>()

const form = defineModel<OrderEditDto>('form', { required: true })

const emit = defineEmits<{
  (e: 'submit'): void
  (e: 'cancel'): void
}>()
</script>

<template>
  <form @submit.prevent="emit('submit')">
    <div class="card">
      <div class="card-header">受注情報</div>

      <div class="card-body">
        <div class="row mb-3">
          <div class="col-md-3">
            <label class="form-label">受注番号</label>
            <input
              :value="form.orderNumber || '登録時に採番されます'"
              class="form-control"
              readonly
            />
          </div>

          <div class="col-md-3">
            <label class="form-label">受注日</label>
            <input
              v-model="form.orderDate"
              type="date"
              class="form-control"
              :class="{ 'is-invalid': errors.orderDate?.[0] }"
            />
            <div class="invalid-feedback">
              {{ errors.orderDate?.[0] }}
            </div>
          </div>

          <div class="col-md-6">
            <label class="form-label">顧客</label>
            <select
              v-model="form.customerId"
              class="form-select"
              :class="{ 'is-invalid': errors.customerId?.[0] }"
            >
              <option value="">選択してください</option>
              <option v-for="customer in customers" :key="customer.id" :value="customer.id">
                {{ customer.displayName ?? `${customer.code} ${customer.name}` }}
              </option>
            </select>
            <div class="invalid-feedback">
              {{ errors.customerId?.[0] }}
            </div>
          </div>
        </div>

        <OrderDetailGrid v-model:form="form" :products="products" :errors="errors" />

        <div class="d-flex justify-content-end gap-2 mt-3">
          <button type="button" class="btn btn-secondary" @click="emit('cancel')">
            キャンセル
          </button>

          <button type="submit" class="btn btn-primary" :disabled="isSubmitting">保存</button>
        </div>
      </div>
    </div>
  </form>
</template>
