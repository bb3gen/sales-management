export interface CreateOrderRequest {
  orderDate: string | null
  customerId: string | null
  details: CreateOrderDetailRequest[]
}

export interface CreateOrderDetailRequest {
  productId: string | null
  quantity: number
}
