<script setup lang="ts">
const customerCode = defineModel<string>('customerCode')
const customerName = defineModel<string>('customerName')
const customerKana = defineModel<string>('customerKana')
const postalCode = defineModel<string>('postalCode')
const address = defineModel<string>('address')
const phoneNumber = defineModel<string>('phoneNumber')
const email = defineModel<string>('email')

defineProps<{
  isEdit?: boolean
  validationErrors?: Record<string, string[]>
}>()
</script>

<template>
  <div class="card">
    <div class="card-body">
      <div v-if="isEdit" class="mb-3">
        <label class="form-label"> 顧客コード </label>

        <input :value="customerCode" class="form-control" readonly />
      </div>

      <div class="mb-3">
        <label class="form-label"> 顧客名 </label>

        <input
          v-model="customerName"
          class="form-control"
          :class="{
            'is-invalid': validationErrors?.CustomerName?.length,
          }"
        />

        <div class="invalid-feedback">
          {{ validationErrors?.CustomerName?.[0] }}
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label"> 顧客名カナ </label>

        <input
          v-model="customerKana"
          class="form-control"
          :class="{
            'is-invalid': validationErrors?.CustomerKana?.length,
          }"
        />

        <div class="invalid-feedback">
          {{ validationErrors?.CustomerKana?.[0] }}
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label"> 郵便番号 </label>

        <input
          v-model="postalCode"
          class="form-control"
          :class="{
            'is-invalid': validationErrors?.PostalCode?.length,
          }"
        />

        <div class="invalid-feedback">
          {{ validationErrors?.PostalCode?.[0] }}
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label"> 住所 </label>

        <input
          v-model="address"
          class="form-control"
          :class="{
            'is-invalid': validationErrors?.Address?.length,
          }"
        />

        <div class="invalid-feedback">
          {{ validationErrors?.Address?.[0] }}
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label"> 電話番号 </label>

        <input
          v-model="phoneNumber"
          class="form-control"
          :class="{
            'is-invalid': validationErrors?.PhoneNumber?.length,
          }"
        />

        <div class="invalid-feedback">
          {{ validationErrors?.PhoneNumber?.[0] }}
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label"> メールアドレス </label>

        <input
          v-model="email"
          class="form-control"
          :class="{
            'is-invalid': validationErrors?.Email?.length,
          }"
        />

        <div class="invalid-feedback">
          {{ validationErrors?.Email?.[0] }}
        </div>
      </div>
    </div>
  </div>
</template>
