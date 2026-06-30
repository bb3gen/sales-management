import type { OrderDetailDto } from './OrderDetailDto'

export interface OrderEditDto {
  id: string
  orderNumber: string
  orderDate: string
  customerId: string
  totalAmount: number
  status: number
  updatedAt: string
  details: OrderDetailDto[]
}
