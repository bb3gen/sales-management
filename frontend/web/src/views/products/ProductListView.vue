<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { getProducts, deleteProduct } from '@/api/productApi'
import type { ProductListItemDto } from '@/types/products/productListItemDto'

const products = ref<ProductListItemDto[]>([])

const keyword = ref('')

const totalCount = ref(0) // 該当件数
const page = ref(1) //ページ数
const pageSize = ref(10) //明細の最大表示件数

// 検索処理
const search = async () => {
  const result = await getProducts({
    keyword: keyword.value,
    page: page.value,
    pageSize: pageSize.value,
  })

  products.value = result.items

  totalCount.value = result.totalCount
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

// 削除
const remove = async (id: string) => {
  const result = confirm('削除しますか？')

  if (!result) {
    return
  }

  await deleteProduct(id)

  if (products.value.length === 1 && page.value > 1) {
    page.value--
  }

  await search()
}

onMounted(search)
</script>

<template>
  <div>
    <h1>商品一覧</h1>

    <div class="mb-3">
      <RouterLink to="/products/create" class="btn btn-primary"> 新規登録 </RouterLink>
    </div>

    <div class="card mb-3">
      <div class="card-body">
        <div class="row">
          <div class="col-md-3">
            <label class="form-label"> キーワード </label>
            <input v-model="keyword" class="form-control" />
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
          <th>商品コード</th>
          <th>商品名</th>
          <th>操作</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="product in products" :key="product.id">
          <td>{{ product.productCode }}</td>
          <td>{{ product.productName }}</td>
          <td>
            <RouterLink
              :to="`/products/${product.id}/edit`"
              class="btn btn-sm btn-outline-primary me-2"
            >
              編集
            </RouterLink>

            <button class="btn btn-sm btn-outline-danger" @click="remove(product.id)">削除</button>
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
