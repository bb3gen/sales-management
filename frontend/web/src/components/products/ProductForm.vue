<script setup lang="ts">
import { computed } from 'vue'

defineProps<{
  productCode?: string
  validationErrors?: Record<string, string[]>
}>()

const emit = defineEmits<{
  (e: 'submit'): void
}>()

const productName = defineModel<string>('productName', { required: true })
const unitPrice = defineModel<number | null>('unitPrice', { required: true })
const unit = defineModel<string>('unit', { default: '' })
const remarks = defineModel<string>('remarks', { default: '' })

const unitPriceInput = computed({
  get: () => unitPrice.value?.toString() ?? '',
  set: (value) => {
    unitPrice.value = value === '' ? null : Number(value)
  },
})
</script>

<template>
  <form @submit.prevent="emit('submit')">
    <div class="mb-3">
      <label class="form-label">商品コード</label>
      <input class="form-control" :value="productCode" readonly />
    </div>

    <div class="mb-3">
      <label class="form-label">商品名</label>
      <input
        v-model="productName"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.productName?.length,
        }"
      />

      <div v-if="validationErrors?.productName" class="invalid-feedback">
        {{ validationErrors.productName[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">単価</label>
      <input
        v-model.number="unitPriceInput"
        type="number"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.unitPrice?.length,
        }"
      />

      <div v-if="validationErrors?.unitPrice" class="invalid-feedback">
        {{ validationErrors.unitPrice[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">単位</label>

      <input
        v-model="unit"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.unit?.length,
        }"
      />

      <div v-if="validationErrors?.unit" class="invalid-feedback">
        {{ validationErrors.unit[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label">備考</label>

      <textarea
        v-model="remarks"
        class="form-control"
        rows="3"
        :class="{
          'is-invalid': validationErrors?.remarks?.length,
        }"
      />

      <div v-if="validationErrors?.remarks" class="invalid-feedback">
        {{ validationErrors.remarks[0] }}
      </div>
    </div>

    <button type="submit" class="btn btn-primary">保存</button>
  </form>
</template>
