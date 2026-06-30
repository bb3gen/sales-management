import { apiClient } from './axios'
import type { PagedResult } from '@/types/common/pagedResult'
import type { CreateOrderRequest } from '@/types/orders/createOrderRequest'
import type { OrderEditDto } from '@/types/orders/OrderEditDto'
import type { OrderListItemDto } from '@/types/orders/orderListItemDto'
import type { OrderSearchRequest } from '@/types/orders/orderSearchRequest'
import type { UpdateOrderRequest } from '@/types/orders/updateOrderRequest'

export const getOrders = async (request: OrderSearchRequest) => {
  const response = await apiClient.get<PagedResult<OrderListItemDto>>('/orders', {
    params: request,
  })
  return response.data
}

export const createOrder = async (request: CreateOrderRequest) => {
  const response = await apiClient.post('/orders', request)
  return response.data
}

export const getOrder = async (id: string) => {
  const response = await apiClient.get<OrderEditDto>(`/orders/${id}`)
  return response.data
}

export const updateOrder = async (id: string, request: UpdateOrderRequest) => {
  await apiClient.put(`/orders/${id}`, request)
}

export const deleteOrder = async (id: string) => {
  await apiClient.delete(`/orders/${id}`)
}
