export interface UpdateOrderRequest {
  orderDate: string | null
  customerId: string | null
  details: UpdateOrderDetailRequest[]
  updatedAt: string
}

export interface UpdateOrderDetailRequest {
  productId: string | null
  quantity: number
}
