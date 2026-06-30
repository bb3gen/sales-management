<script setup lang="ts">
defineProps<{
  validationErrors?: Record<string, string[]>
}>()

const emit = defineEmits<{
  (e: 'submit'): void
}>()

const customerCode = defineModel<string>('customerCode', { default: '' })
const customerName = defineModel<string>('customerName', { required: true })
const customerKana = defineModel<string>('customerKana', { default: '' })
const postalCode = defineModel<string>('postalCode', { default: '' })
const address = defineModel<string>('address', { default: '' })
const phoneNumber = defineModel<string>('phoneNumber', { default: '' })
const email = defineModel<string>('email', { default: '' })
</script>
<template>
  <form @submit.prevent="emit('submit')">
    <div class="mb-3">
      <label class="form-label"> 顧客コード </label>
      <input :value="customerCode" class="form-control" readonly />
    </div>

    <div class="mb-3">
      <label class="form-label"> 顧客名 </label>
      <input
        v-model="customerName"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.customerName?.length,
        }"
      />
      <div v-if="validationErrors?.customerName" class="invalid-feedback">
        {{ validationErrors.customerName[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label"> 顧客名カナ </label>
      <input
        v-model="customerKana"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.customerKana?.length,
        }"
      />
      <div v-if="validationErrors?.customerKana" class="invalid-feedback">
        {{ validationErrors.customerKana[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label"> 郵便番号 </label>
      <input
        v-model="postalCode"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.postalCode?.length,
        }"
      />
      <div v-if="validationErrors?.postalCode" class="invalid-feedback">
        {{ validationErrors.postalCode[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label"> 住所 </label>
      <input
        v-model="address"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.address?.length,
        }"
      />
      <div v-if="validationErrors?.address" class="invalid-feedback">
        {{ validationErrors.address[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label"> 電話番号 </label>
      <input
        v-model="phoneNumber"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.phoneNumber?.length,
        }"
      />
      <div v-if="validationErrors?.phoneNumber" class="invalid-feedback">
        {{ validationErrors.phoneNumber[0] }}
      </div>
    </div>

    <div class="mb-3">
      <label class="form-label"> メールアドレス </label>
      <input
        v-model="email"
        class="form-control"
        :class="{
          'is-invalid': validationErrors?.email?.length,
        }"
      />
      <div v-if="validationErrors?.email" class="invalid-feedback">
        {{ validationErrors.email[0] }}
      </div>
    </div>

    <button type="submit" class="btn btn-primary">保存</button>
  </form>
</template>
