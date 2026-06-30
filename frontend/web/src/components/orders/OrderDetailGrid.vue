<script setup lang="ts">
import { createEmptyOrderDetail } from '@/types/orders/createEmptyOrderDetail'
import type { OrderDetailDto } from '@/types/orders/OrderDetailDto'
import type { OrderEditDto } from '@/types/orders/OrderEditDto'
import type { ProductLookupDto } from '@/types/products/productLookupDto'
import type { ValidationErrors } from '@/types/common/ValidationErrors'

interface Props {
  products: ProductLookupDto[]
  errors: ValidationErrors
}

const props = defineProps<Props>()
const form = defineModel<OrderEditDto>('form', { required: true })

const updateForm = (details: OrderDetailDto[]) => {
  form.value = {
    ...form.value,
    details,
    totalAmount: details.reduce((sum, detail) => sum + detail.amount, 0),
  }
}

const updateDetail = (index: number, detail: OrderDetailDto) => {
  updateForm(form.value.details.map((current, i) => (i === index ? detail : current)))
}

const addRow = () => {
  updateForm([...form.value.details, createEmptyOrderDetail(form.value.details.length + 1)])
}

const removeRow = (index: number) => {
  updateForm(
    form.value.details
      .filter((_, i) => i !== index)
      .map((detail, i) => ({
        ...detail,
        lineNo: i + 1,
      })),
  )
}

const productChanged = (index: number, event: Event) => {
  const detail = form.value.details[index]
  const productId = (event.target as HTMLSelectElement).value

  if (!detail) {
    return
  }

  const product = props.products.find((x) => x.id === productId)

  if (!product) {
    updateDetail(index, {
      ...detail,
      productId,
      productCode: '',
      productName: '',
      unit: '',
      unitPrice: 0,
      amount: 0,
    })
    return
  }

  updateDetail(index, {
    ...detail,
    productId,
    productCode: product.code,
    productName: product.name,
    unit: product.unit ?? '',
    unitPrice: product.unitPrice,
    amount: detail.quantity * product.unitPrice,
  })
}

const quantityChanged = (index: number, event: Event) => {
  const detail = form.value.details[index]
  const quantity = Number((event.target as HTMLInputElement).value)

  if (!detail) {
    return
  }

  updateDetail(index, {
    ...detail,
    quantity,
    amount: quantity * detail.unitPrice,
  })
}
</script>

<template>
  <div class="card mt-3">
    <div class="card-header">明細</div>

    <div class="card-body">
      <div v-if="errors.details?.[0]" class="text-danger small mb-2">
        {{ errors.details[0] }}
      </div>

      <table class="table table-bordered align-middle">
        <thead>
          <tr>
            <th style="width: 40%">商品</th>
            <th style="width: 10%">数量</th>
            <th style="width: 10%">単位</th>
            <th style="width: 15%">単価</th>
            <th style="width: 15%">金額</th>
            <th style="width: 10%"></th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="(detail, index) in form.details" :key="index">
            <td>
              <select
                :value="detail.productId"
                class="form-select"
                :class="{ 'is-invalid': errors[`details[${index}].productId`]?.[0] }"
                @change="productChanged(index, $event)"
              >
                <option value="">選択してください</option>

                <option v-for="product in products" :key="product.id" :value="product.id">
                  {{ product.displayName }}
                </option>
              </select>

              <div class="invalid-feedback">
                {{ errors[`details[${index}].productId`]?.[0] }}
              </div>
            </td>

            <td>
              <input
                :value="detail.quantity"
                type="number"
                min="1"
                class="form-control text-end"
                :class="{ 'is-invalid': errors[`details[${index}].quantity`]?.[0] }"
                @input="quantityChanged(index, $event)"
              />

              <div class="invalid-feedback">
                {{ errors[`details[${index}].quantity`]?.[0] }}
              </div>
            </td>

            <td class="text-center">
              {{ detail.unit }}
            </td>

            <td class="text-end">
              {{ detail.unitPrice.toLocaleString() }}
            </td>

            <td class="text-end">
              {{ detail.amount.toLocaleString() }}
            </td>

            <td class="text-center">
              <button type="button" class="btn btn-outline-danger btn-sm" @click="removeRow(index)">
                削除
              </button>
            </td>
          </tr>
        </tbody>

        <tfoot>
          <tr>
            <td colspan="4" class="text-end">
              <strong>合計</strong>
            </td>

            <td class="text-end">
              <strong>
                {{ form.totalAmount.toLocaleString() }}
              </strong>
            </td>

            <td></td>
          </tr>
        </tfoot>
      </table>

      <button type="button" class="btn btn-outline-primary" @click="addRow">＋ 明細追加</button>
    </div>
  </div>
</template>
