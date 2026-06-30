import type { OrderEditDto } from './OrderEditDto'
import { createEmptyOrderDetail } from './createEmptyOrderDetail'

export function createEmptyOrder(): OrderEditDto {
  return {
    id: '',

    orderNumber: '',

    orderDate: new Date().toISOString().slice(0, 10),

    customerId: '',

    totalAmount: 0,

    status: 0,

    updatedAt: '',

    details: [createEmptyOrderDetail(1)],
  }
}
