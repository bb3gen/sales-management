<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { deleteOrder, getOrders } from '@/api/orderApi'
import { getCustomerLookup } from '@/api/customerApi'
import type { LookupItemDto } from '@/types/common/lookupItemDto'
import type { OrderListItemDto } from '@/types/orders/orderListItemDto'

const orders = ref<OrderListItemDto[]>([])
const customers = ref<LookupItemDto[]>([])

const orderNumber = ref('')
const customerId = ref('')
const orderDateFrom = ref('')
const orderDateTo = ref('')
const status = ref('')

const totalCount = ref(0) // 該当件数
const page = ref(1) // ページ数
const pageSize = ref(10) // 明細の最大表示件数

const statusLabels: Record<number, string> = {
  0: '下書き',
  1: '確定',
  9: 'キャンセル',
}

// 検索処理
const search = async () => {
  const result = await getOrders({
    page: page.value,
    pageSize: pageSize.value,
    orderNumber: orderNumber.value || undefined,
    customerId: customerId.value || undefined,
    orderDateFrom: orderDateFrom.value || undefined,
    orderDateTo: orderDateTo.value || undefined,
    status: status.value === '' ? undefined : Number(status.value),
  })

  orders.value = result.items
  totalCount.value = result.totalCount
}

const loadCustomers = async () => {
  customers.value = await getCustomerLookup()
}

// 総ページ数
const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value))

// 前へ
const previousPage = async () => {
  if (page.value <= 1) {
    return
  }

  page.value--

  await search()
}

// 次へ
const nextPage = async () => {
  if (page.value >= totalPages.value) {
    return
  }

  page.value++

  await search()
}

// 検索ボタン
const executeSearch = async () => {
  page.value = 1

  await search()
}

const formatAmount = (value: number) => {
  return value.toLocaleString()
}

const formatStatus = (status: number) => {
  return statusLabels[status] ?? status.toString()
}

// 削除
const remove = async (id: string) => {
  const result = confirm('削除しますか？')

  if (!result) {
    return
  }

  await deleteOrder(id)

  if (orders.value.length === 1 && page.value > 1) {
    page.value--
  }

  await search()
}

onMounted(async () => {
  await loadCustomers()
  await search()
})
</script>

<template>
  <div>
    <h1>注文一覧</h1>

    <div class="mb-3">
      <RouterLink to="/orders/create" class="btn btn-primary"> 新規登録 </RouterLink>
    </div>

    <div class="card mb-3">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-3">
            <label class="form-label">受注番号</label>
            <input v-model="orderNumber" class="form-control" />
          </div>

          <div class="col-md-3">
            <label class="form-label">顧客</label>
            <select v-model="customerId" class="form-select">
              <option value="">すべて</option>
              <option v-for="customer in customers" :key="customer.id" :value="customer.id">
                {{ customer.displayName ?? `${customer.code} ${customer.name}` }}
              </option>
            </select>
          </div>

          <div class="col-md-2">
            <label class="form-label">受注日From</label>
            <input v-model="orderDateFrom" type="date" class="form-control" />
          </div>

          <div class="col-md-2">
            <label class="form-label">受注日To</label>
            <input v-model="orderDateTo" type="date" class="form-control" />
          </div>

          <div class="col-md-2">
            <label class="form-label">ステータス</label>
            <select v-model="status" class="form-select">
              <option value="">すべて</option>
              <option value="0">下書き</option>
              <option value="1">確定</option>
              <option value="9">キャンセル</option>
            </select>
          </div>
        </div>

        <div class="mt-3">
          <button class="btn btn-primary" @click="executeSearch">検索</button>
        </div>
      </div>
    </div>

    <table class="table table-striped">
      <thead>
        <tr>
          <th>受注番号</th>
          <th>受注日</th>
          <th>顧客名</th>
          <th class="text-end">合計金額</th>
          <th>ステータス</th>
          <th>操作</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="order in orders" :key="order.id">
          <td>{{ order.orderNumber }}</td>
          <td>{{ order.orderDate }}</td>
          <td>{{ order.customerName }}</td>
          <td class="text-end">{{ formatAmount(order.totalAmount) }}</td>
          <td>{{ formatStatus(order.status) }}</td>
          <td>
            <RouterLink
              :to="`/orders/${order.id}/edit`"
              class="btn btn-sm btn-outline-primary me-2"
            >
              編集
            </RouterLink>

            <button class="btn btn-sm btn-outline-danger" @click="remove(order.id)">削除</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div class="d-flex justify-content-between align-items-center mt-3">
      <button class="btn btn-outline-secondary" :disabled="page <= 1" @click="previousPage">
        前へ
      </button>

      <div>{{ page }} / {{ totalPages }} （{{ totalCount }}件）</div>

      <button class="btn btn-outline-secondary" :disabled="page >= totalPages" @click="nextPage">
        次へ
      </button>
    </div>
  </div>
</template>
