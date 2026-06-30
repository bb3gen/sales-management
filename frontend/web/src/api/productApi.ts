import { apiClient } from './axios'
import type { PagedResult } from '@/types/common/pagedResult'
import type { ProductListItemDto } from '@/types/products/productListItemDto'
import type { ProductSearchRequest } from '@/types/products/productSearchRequest'
import type { CreateProductRequest } from '@/types/products/createProductRequest'
import type { UpdateProductRequest } from '@/types/products/updateProductRequest'
import type { ProductEditDto } from '@/types/products/productEditDto'
import type { ProductLookupDto } from '@/types/products/productLookupDto'


// 商品リスト検索
export const getProducts = async (request: ProductSearchRequest) => {
  const response = await apiClient.get<PagedResult<ProductListItemDto>>('/products', {
    params: request,
  })
  return response.data
}

// 商品取得
export const getProduct = async (id: string) => {
  const response = await apiClient.get<ProductEditDto>(`/products/${id}`)
  return response.data
}

// 商品登録
export const createProduct = async (request: CreateProductRequest) => {
  const response = await apiClient.post('/products', request)
  return response.data
}

// 商品更新
export const updateProduct = async (id: string, request: UpdateProductRequest) => {
  await apiClient.put(`/products/${id}`, request)
}

// 商品削除
export const deleteProduct = async (id: string) => {
  await apiClient.delete(`/products/${id}`)
}

export const getProductLookup = async (): Promise<ProductLookupDto[]> => {
  const response = await apiClient.get<ProductLookupDto[]>('/products/lookup')
  return response.data
}
