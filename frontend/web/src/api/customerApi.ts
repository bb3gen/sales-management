import { apiClient } from './axios'
import type { PagedResult } from '@/types/common/pagedResult'
import type { Customer } from '@/types/customers/customer'
import type { CustomerSearchRequest } from '@/types/customers/customerSearchRequest'
import type { CreateCustomerRequest } from '@/types/customers/createCustomerRequest'
import type { UpdateCustomerRequest } from '@/types/customers/updateCustomerRequest'

// 顧客リスト検索
export const getCustomers = async (request: CustomerSearchRequest) => {
  const response = await apiClient.get<PagedResult<Customer>>('/customers', {
    params: request,
  })

  return response.data
}

// 顧客追加
export const createCustomer = async (request: CreateCustomerRequest) => {
  const response = await apiClient.post('/customers', request)

  return response.data
}

// 顧客取得
export const getCustomer = async (id: string) => {
  const response = await apiClient.get(`/customers/${id}`)

  return response.data
}

// 顧客更新
export const updateCustomer = async (id: string, request: UpdateCustomerRequest) => {
  await apiClient.put(`/customers/${id}`, request)
}

// 顧客削除
export const deleteCustomer = async (id: string) => {
  await apiClient.delete(`/customers/${id}`)
}
